using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceController : MonoBehaviour
{
    private static readonly List<PoliceController> activePolice = new List<PoliceController>();

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 8f;
    [SerializeField] private float catchDistance = 0.15f;
    [SerializeField] private bool moveOnlyByX = true;
    [Tooltip("Сколько секунд полиция стоит в idle/поимке после добегания до точки, прежде чем день завершится.")]
    [SerializeField] private float catchIdleDelay = 3f;

    [Header("Devil Advocate")]
    [Tooltip("Если адвокат дьявола отменил поимку, полиция всё равно добегает до точки, но НЕ ловит игрока, а продолжает идти дальше к выходу.")]
    [SerializeField] private bool continuePatrolAfterDevilAdvocateBlock = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string runBoolName = "IsRunning";
    [Tooltip("Необязательно. Если в Animator есть bool для idle, укажи его здесь. Если пусто — достаточно выключения IsRunning.")]
    [SerializeField] private string idleBoolName = "";

    [Header("Visual Objects")]
    [Tooltip("Объект/картинка/анимация бега полиции. Например Run.")]
    [SerializeField] private GameObject runVisualObject;
    [Tooltip("Обычная idle-картинка полиции. Можно оставить пустым.")]
    [SerializeField] private GameObject idleVisualObject;
    [Tooltip("Картинка/поза поимки, которая включается после добегания полиции до точки.")]
    [SerializeField] private GameObject catchVisualObject;
    [Tooltip("Эмоджи/смайлик/значок над полицией при поимке. Можно оставить пустым.")]
    [SerializeField] private GameObject catchEmojiObject;
    [SerializeField] private bool useVisualObjectSwitching = true;

    [Header("Flip")]
    [SerializeField] private Transform visualRootToFlip;
    [SerializeField] private bool positiveScaleXFacingRight = true;

    private Coroutine chaseCoroutine;
    private int lastMoveDirection = -1;

    private void OnEnable()
    {
        if (!activePolice.Contains(this))
            activePolice.Add(this);
    }

    private void OnDisable()
    {
        activePolice.Remove(this);
    }

    public static bool StartClosestChase(Transform target, Action onCaught)
    {
        PoliceController closest = FindClosestToTarget(target);
        if (closest == null)
            return false;

        closest.StartChase(target, onCaught);
        return true;
    }

    public static bool StartClosestDevilAdvocateBlockRun(Transform target)
    {
        PoliceController closest = FindClosestToTarget(target);
        if (closest == null)
            return false;

        closest.StartDevilAdvocateBlockRun(target);
        return true;
    }

    private static PoliceController FindClosestToTarget(Transform target)
    {
        activePolice.RemoveAll(item => item == null || !item.isActiveAndEnabled);

        if (target == null || activePolice.Count == 0)
            return null;

        PoliceController closest = null;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < activePolice.Count; i++)
        {
            float distance = Mathf.Abs(activePolice[i].transform.position.x - target.position.x);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                closest = activePolice[i];
            }
        }

        return closest;
    }

    public void StartChase(Transform target, Action onCaught)
    {
        PolicePatrol patrol = GetComponent<PolicePatrol>();
        if (patrol != null)
            patrol.StopPatrol();

        if (chaseCoroutine != null)
            StopCoroutine(chaseCoroutine);

        chaseCoroutine = StartCoroutine(ChaseRoutine(target, onCaught));
    }

    public void StartDevilAdvocateBlockRun(Transform target)
    {
        PolicePatrol patrol = GetComponent<PolicePatrol>();
        if (patrol != null)
            patrol.StopPatrol();

        if (chaseCoroutine != null)
            StopCoroutine(chaseCoroutine);

        chaseCoroutine = StartCoroutine(DevilAdvocateBlockRoutine(target));
    }

    private IEnumerator ChaseRoutine(Transform target, Action onCaught)
    {
        SetCatchEmoji(false);
        SetVisual(runVisualObject);
        SetIdle(false);
        SetRunning(true);

        yield return MoveToTarget(target);

        // Добежали: выключаем бег, включаем idle/картинку поимки и эмоджи.
        SetRunning(false);
        SetIdle(true);
        SetVisual(catchVisualObject != null ? catchVisualObject : idleVisualObject);
        SetCatchEmoji(true);

        if (catchIdleDelay > 0f)
            yield return new WaitForSeconds(catchIdleDelay);

        onCaught?.Invoke();
    }

    private IEnumerator DevilAdvocateBlockRoutine(Transform target)
    {
        // Адвокат отменил поимку: полиция подбегает, но не включает CatchVisual,
        // не завершает день и после точки продолжает идти дальше к выходу.
        SetCatchEmoji(false);
        SetIdle(false);
        SetVisual(runVisualObject);
        SetRunning(true);

        yield return MoveToTarget(target);

        SetCatchEmoji(false);
        SetIdle(false);
        SetVisual(runVisualObject);
        SetRunning(true);

        if (continuePatrolAfterDevilAdvocateBlock)
        {
            PolicePatrol patrol = GetComponent<PolicePatrol>();
            if (patrol != null)
            {
                patrol.SetDirection(lastMoveDirection);
                patrol.StartPatrol();
            }
        }

        chaseCoroutine = null;
    }

    private IEnumerator MoveToTarget(Transform target)
    {
        while (target != null)
        {
            Vector3 pos = transform.position;
            Vector3 targetPos = target.position;

            float dx = targetPos.x - pos.x;
            if (Mathf.Abs(dx) > 0.001f)
                lastMoveDirection = dx > 0f ? 1 : -1;

            FaceByDirection(dx);

            if (moveOnlyByX)
            {
                float newX = Mathf.MoveTowards(pos.x, targetPos.x, chaseSpeed * Time.deltaTime);
                transform.position = new Vector3(newX, pos.y, pos.z);

                if (Mathf.Abs(targetPos.x - transform.position.x) <= catchDistance)
                    break;
            }
            else
            {
                transform.position = Vector3.MoveTowards(pos, targetPos, chaseSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, targetPos) <= catchDistance)
                    break;
            }

            yield return null;
        }
    }

    private void SetVisual(GameObject targetVisual)
    {
        if (!useVisualObjectSwitching)
            return;

        if (targetVisual == null)
            targetVisual = idleVisualObject != null ? idleVisualObject : runVisualObject;

        SetVisualObject(runVisualObject, targetVisual);
        SetVisualObject(idleVisualObject, targetVisual);
        SetVisualObject(catchVisualObject, targetVisual);
    }

    private void SetVisualObject(GameObject visual, GameObject targetVisual)
    {
        if (visual == null)
            return;

        visual.SetActive(visual == targetVisual);
    }

    private void SetCatchEmoji(bool value)
    {
        if (catchEmojiObject != null)
            catchEmojiObject.SetActive(value);
    }

    private void SetRunning(bool value)
    {
        if (animator != null && !string.IsNullOrEmpty(runBoolName))
            animator.SetBool(runBoolName, value);
    }

    private void SetIdle(bool value)
    {
        if (animator != null && !string.IsNullOrEmpty(idleBoolName))
            animator.SetBool(idleBoolName, value);
    }

    private void FaceByDirection(float directionX)
    {
        if (visualRootToFlip == null)
            return;

        if (Mathf.Abs(directionX) < 0.001f)
            return;

        bool shouldFaceRight = directionX > 0f;
        Vector3 scale = visualRootToFlip.localScale;
        float absX = Mathf.Max(0.0001f, Mathf.Abs(scale.x));

        bool positiveNeeded = positiveScaleXFacingRight ? shouldFaceRight : !shouldFaceRight;
        scale.x = positiveNeeded ? absX : -absX;
        visualRootToFlip.localScale = scale;
    }
}
