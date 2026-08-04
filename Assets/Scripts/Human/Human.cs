using System.Collections;
using UnityEngine;

public abstract class Human : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string humanType = "worker";
    public string[] needReaction;
    protected abstract string[] GetReactions();

    [Header("Old public fields compatibility")]
    public GameObject background;
    public GameObject[] emotion;
    public string Name;
    public int reactionState; // 2 = придёт на мессу, 1 = не придёт
    public bool interact = true;

    [Header("Movement")]
    [SerializeField] private GameObject walkVisualObject;
    [SerializeField] private GameObject idleVisualObject;
    [SerializeField] private Animator walkAnimator;
    [SerializeField] private string walkingBoolName = "IsWalking";

    [Header("Reaction Idle Visuals")]
    [Tooltip("Поза/картинка персонажа при правильной листовке. Это НЕ смайлик, а визуал самого персонажа вместо обычного IdleVisual.")]
    [SerializeField] private GameObject happyIdleVisualObject;
    [Tooltip("Поза/картинка персонажа при неправильной листовке. Это НЕ смайлик, а визуал самого персонажа вместо обычного IdleVisual.")]
    [SerializeField] private GameObject negativeIdleVisualObject;

    [Header("Direction flip")]
    [SerializeField] private bool useSimpleDirectionFlip = true;
    [SerializeField] private Transform visualRootToFlip;
    [SerializeField] private bool positiveScaleXFacingRight = true;
    [SerializeField] private float streetMoveDirectionX = -1f;
    [SerializeField] private bool changeOnlyScaleXSign = true;

    [Header("Flyer hit")]
    [SerializeField] private Transform facePoint;
    [SerializeField] private SpriteRenderer bubbleFlyerIcon;
    [SerializeField] private float flyerInBubbleTime = 0.35f;
    [SerializeField] private float stopAfterHitTime = 0.35f;

    [Header("Messa door movement")]
    [SerializeField] private Transform doorTargetOverride;
    [SerializeField] private float messaDoorMoveSpeed = 7f;
    [SerializeField] private float doorStopDistance = 0.05f;

    [Header("Police")]
    [SerializeField] private GameObject policeWarningIcon;

    private bool canMove = true;
    private bool alreadyReceivedFlyer;
    private bool waitingForFlyerHit;
    private bool goingToMessa;
    private Coroutine receiveFlyerCoroutine;
    private Rigidbody2D cachedRigidbody2D;

    private bool hasWantedFacing;
    private bool wantedFaceRight;
    private float safeAbsScaleX = 1f;
    private int currentIdleVisualState; // 0 = обычный idle, 1 = negative, 2 = happy

    public string HumanType => string.IsNullOrEmpty(humanType) ? Name : humanType;
    public bool IsGoingToMessa => goingToMessa;
    public bool AlreadyReceivedFlyer => alreadyReceivedFlyer;
    public bool IsInPoliceZone { get; private set; }

    protected virtual void Awake()
    {
        cachedRigidbody2D = GetComponent<Rigidbody2D>();
        needReaction = GetReactions();

        if (string.IsNullOrEmpty(humanType))
            humanType = Name;

        Transform flipTarget = GetFlipTarget();
        if (flipTarget != null)
            safeAbsScaleX = Mathf.Max(0.0001f, Mathf.Abs(flipTarget.localScale.x));

        HideBubbleFlyerIcon();
        HideReactionIdleVisuals();
        SetPoliceZone(false);
        SetCanMove(true);
        FaceStreetDirection();
    }

    private void LateUpdate()
    {
        ApplyWantedFacing();
    }

    public void SetHumanType(string value)
    {
        humanType = value;
    }

    public void SetPoliceZone(bool value)
    {
        IsInPoliceZone = value;
        if (policeWarningIcon != null)
            policeWarningIcon.SetActive(value);
    }

    public bool CanMove()
    {
        return canMove;
    }

    private void SetCanMove(bool value)
    {
        canMove = value;
        UpdateWalkIdleVisual();
    }

    private void UpdateWalkIdleVisual()
    {
        // Важно: нельзя оставить все визуалы выключенными.
        // Если Happy/Negative не назначены, используем обычный Idle как fallback.
        GameObject targetVisual;

        if (canMove)
        {
            targetVisual = walkVisualObject != null ? walkVisualObject : idleVisualObject;
        }
        else
        {
            if (currentIdleVisualState == 2 && happyIdleVisualObject != null)
                targetVisual = happyIdleVisualObject;
            else if (currentIdleVisualState == 1 && negativeIdleVisualObject != null)
                targetVisual = negativeIdleVisualObject;
            else if (idleVisualObject != null)
                targetVisual = idleVisualObject;
            else
                targetVisual = walkVisualObject;
        }

        SetVisualObject(walkVisualObject, targetVisual);
        SetVisualObject(idleVisualObject, targetVisual);
        SetVisualObject(happyIdleVisualObject, targetVisual);
        SetVisualObject(negativeIdleVisualObject, targetVisual);

        if (walkAnimator != null && !string.IsNullOrEmpty(walkingBoolName))
            walkAnimator.SetBool(walkingBoolName, canMove);
    }

    private void SetVisualObject(GameObject visual, GameObject targetVisual)
    {
        if (visual == null)
            return;

        visual.SetActive(visual == targetVisual);
    }

    private void SetIdleVisualState(int state)
    {
        currentIdleVisualState = state;
        UpdateWalkIdleVisual();
    }

    private void HideReactionIdleVisuals()
    {
        currentIdleVisualState = 0;

        if (negativeIdleVisualObject != null)
            negativeIdleVisualObject.SetActive(false);

        if (happyIdleVisualObject != null)
            happyIdleVisualObject.SetActive(false);
    }

    public bool CanReceiveFlyer()
    {
        return interact && !alreadyReceivedFlyer && !waitingForFlyerHit;
    }

    public bool CanBeMegafonRecruited()
    {
        return !alreadyReceivedFlyer && !waitingForFlyerHit;
    }

    public bool ReserveForFlyerThrow()
    {
        if (!CanReceiveFlyer())
            return false;

        // IMPORTANT: reserving target does NOT stop the NPC.
        // The NPC keeps walking while the flyer is flying.
        // Stop + Idle/Happy/Negative reaction happens only after FlyingFlyer hits FacePoint.
        waitingForFlyerHit = true;
        interact = false;

        if (background != null)
            background.SetActive(false);

        HideAllEmotions();
        return true;
    }

    public void CancelFlyerReservation()
    {
        if (alreadyReceivedFlyer)
            return;

        waitingForFlyerHit = false;
        interact = true;
        SetIdleVisualState(0);
        SetCanMove(true);
        FaceStreetDirection();

        if (background != null)
            background.SetActive(false);
    }

    public Vector3 GetFacePosition()
    {
        if (facePoint != null)
            return facePoint.position;

        return transform.position + new Vector3(0f, 0.6f, 0f);
    }

    public void ReceiveFlyer(string reaction, Sprite bubbleSprite)
    {
        if (alreadyReceivedFlyer)
            return;

        if (!waitingForFlyerHit)
        {
            if (!CanReceiveFlyer())
                return;

            ReserveForFlyerThrow();
        }

        StartReceiveFlyerRoutine(reaction, bubbleSprite);
    }

    public void ReceiveFlyerFromMegafon(string reaction, Sprite bubbleSprite)
    {
        if (!CanBeMegafonRecruited())
            return;

        waitingForFlyerHit = false;
        interact = false;

        if (background != null)
            background.SetActive(false);

        StartReceiveFlyerRoutine(reaction, bubbleSprite);
    }

    public void SetReaction(string reaction)
    {
        ReceiveFlyer(reaction, null);
    }

    private void StartReceiveFlyerRoutine(string reaction, Sprite bubbleSprite)
    {
        if (receiveFlyerCoroutine != null)
            StopCoroutine(receiveFlyerCoroutine);

        receiveFlyerCoroutine = StartCoroutine(ReceiveFlyerRoutine(reaction, bubbleSprite));
    }

    private IEnumerator ReceiveFlyerRoutine(string reaction, Sprite bubbleSprite)
    {
        alreadyReceivedFlyer = true;
        waitingForFlyerHit = false;
        interact = false;

        bool isCorrectReaction = needReaction != null && needReaction.Length > 0 && reaction == needReaction[0];

        // ВАЖНО: реакция включается МГНОВЕННО в момент попадания листовки в FacePoint.
        // Раньше персонаж сначала показывал обычный idle/ещё мог выглядеть как Run,
        // ждал flyerInBubbleTime и только потом включал Happy/Negative. Из-за этого было ощущение задержки.
        StopPhysicsMotion();
        HideAllEmotions();

        if (isCorrectReaction)
        {
            gameObject.layer = 2; // Ignore Raycast, второй раз кинуть нельзя
            reactionState = 2;
            goingToMessa = true;
            SetIdleVisualState(2); // радостная поза персонажа сразу
            FaceDoorDirection();
        }
        else
        {
            reactionState = 1;
            goingToMessa = false;
            SetIdleVisualState(1); // негативная поза персонажа сразу
            FaceStreetDirection();
        }

        SetCanMove(false);
        StopPhysicsMotion();

        if (emotion != null && reactionState >= 0 && reactionState < emotion.Length && emotion[reactionState] != null)
            emotion[reactionState].SetActive(true);

        ShowBubbleFlyerIcon(bubbleSprite);

        if (needReaction != null && needReaction.Length > 0)
            Debug.Log("Тип: " + HumanType + " | Нужно: " + needReaction[0] + " | Дали: " + reaction + " | Результат: " + reactionState + " | В мессу: " + goingToMessa);
        else
            Debug.LogWarning("У прохожего не настроен needReaction. Дали: " + reaction);

        yield return new WaitForSeconds(flyerInBubbleTime);

        HideBubbleFlyerIcon();

        yield return new WaitForSeconds(stopAfterHitTime);

        SetCanMove(true);
        SetIdleVisualState(0);
        receiveFlyerCoroutine = null;
    }

    private void StopPhysicsMotion()
    {
        if (cachedRigidbody2D == null)
            cachedRigidbody2D = GetComponent<Rigidbody2D>();

        if (cachedRigidbody2D != null)
        {
            cachedRigidbody2D.velocity = Vector2.zero;
            cachedRigidbody2D.angularVelocity = 0f;
        }
    }

    protected void MoveHuman(float speed)
    {
        if (!CanMove())
            return;

        if (goingToMessa)
        {
            Transform doorTarget = GetDoorTarget();
            if (doorTarget != null)
            {
                float targetX = doorTarget.position.x;
                float directionX = targetX - transform.position.x;

                FaceByDirectionX(directionX);

                float newX = Mathf.MoveTowards(transform.position.x, targetX, messaDoorMoveSpeed * Time.deltaTime);
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);

                if (Mathf.Abs(targetX - transform.position.x) <= doorStopDistance)
                {
                    SetCanMove(false);
                    if (MessaDoorTrigger.Current != null)
                        MessaDoorTrigger.Current.AcceptHuman(this);
                }

                return;
            }
        }

        FaceStreetDirection();
        float x = Time.deltaTime * speed * Mathf.Abs(streetMoveDirectionX);
        transform.Translate(new Vector2(Mathf.Sign(streetMoveDirectionX) * x, 0f));
    }

    private Transform GetDoorTarget()
    {
        if (doorTargetOverride != null)
            return doorTargetOverride;

        return MessaDoorPoint.Current;
    }

    private void FaceDoorDirection()
    {
        Transform doorTarget = GetDoorTarget();
        if (doorTarget == null)
            return;

        FaceByDirectionX(doorTarget.position.x - transform.position.x);
    }

    private void FaceStreetDirection()
    {
        FaceByDirectionX(streetMoveDirectionX);
    }

    private void FaceByDirectionX(float directionX)
    {
        if (!useSimpleDirectionFlip)
            return;

        if (Mathf.Abs(directionX) < 0.001f)
            return;

        wantedFaceRight = directionX > 0f;
        hasWantedFacing = true;
    }

    private void ApplyWantedFacing()
    {
        if (!useSimpleDirectionFlip || !hasWantedFacing)
            return;

        Transform target = GetFlipTarget();
        if (target == null)
            return;

        Vector3 scale = target.localScale;
        float absX = Mathf.Abs(scale.x);
        if (absX < 0.0001f)
            absX = safeAbsScaleX;

        float sign = positiveScaleXFacingRight == wantedFaceRight ? 1f : -1f;
        scale.x = absX * sign;
        target.localScale = scale;
    }

    private Transform GetFlipTarget()
    {
        return visualRootToFlip;
    }

    private void ShowBubbleFlyerIcon(Sprite sprite)
    {
        if (bubbleFlyerIcon == null)
            return;

        if (sprite != null)
            bubbleFlyerIcon.sprite = sprite;

        bubbleFlyerIcon.enabled = true;
    }

    private void HideBubbleFlyerIcon()
    {
        if (bubbleFlyerIcon != null)
            bubbleFlyerIcon.enabled = false;
    }

    private void HideAllEmotions()
    {
        if (emotion == null)
            return;

        for (int i = 0; i < emotion.Length; i++)
        {
            if (emotion[i] != null)
                emotion[i].SetActive(false);
        }
    }

    public void OnMouseEnter()
    {
        if (background != null && CanReceiveFlyer())
            background.SetActive(true);
    }

    public void OnMouseExit()
    {
        if (background != null)
            background.SetActive(false);
    }
}
