using UnityEngine;

// Обычная ходьба полиции по улице.
// Полиция идёт как прохожий, видит своей зоной, а при максимальном подозрении PoliceController останавливает патруль и запускает погоню.
public class PolicePatrol : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2.8f;
    [SerializeField] private int startDirection = -1; // -1 = left, 1 = right
    [SerializeField] private bool moveOnlyByX = true;

    [Header("Optional Borders")]
    [Tooltip("OFF = полиция просто идёт по улице и удаляется StreetExitTrigger. ON = ходит туда-сюда между границами.")]
    [SerializeField] private bool useBorders = false;
    [SerializeField] private float leftBorderX = -10f;
    [SerializeField] private float rightBorderX = 10f;

    [Header("Visual Flip")]
    [Tooltip("Перетащи сюда объект Run полиции. Скрипт меняет только знак Scale X у Run.")]
    [SerializeField] private Transform visualRootToFlip;
    [SerializeField] private bool positiveScaleXFacingRight = true;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string walkBoolName = "IsWalking";

    private int direction;
    private bool canPatrol = true;

    private void Awake()
    {
        direction = startDirection >= 0 ? 1 : -1;
    }

    private void OnEnable()
    {
        canPatrol = true;
        SetWalkAnimation(true);
        ApplyFlip();
    }

    private void Update()
    {
        if (!canPatrol)
            return;

        Vector3 move = moveOnlyByX ? new Vector3(direction, 0f, 0f) : transform.right * direction;
        transform.position += move.normalized * patrolSpeed * Time.deltaTime;

        if (useBorders)
        {
            if (transform.position.x <= leftBorderX)
            {
                direction = 1;
                ApplyFlip();
            }
            else if (transform.position.x >= rightBorderX)
            {
                direction = -1;
                ApplyFlip();
            }
        }
    }

    public void StopPatrol()
    {
        canPatrol = false;
        SetWalkAnimation(false);
    }

    public void StartPatrol()
    {
        canPatrol = true;
        SetWalkAnimation(true);
        ApplyFlip();
    }

    public void SetDirection(int newDirection)
    {
        direction = newDirection >= 0 ? 1 : -1;
        ApplyFlip();
    }

    private void ApplyFlip()
    {
        if (visualRootToFlip == null)
            return;

        Vector3 scale = visualRootToFlip.localScale;
        float absX = Mathf.Max(0.0001f, Mathf.Abs(scale.x));

        bool movingRight = direction > 0;
        bool positiveNeeded = positiveScaleXFacingRight ? movingRight : !movingRight;

        scale.x = positiveNeeded ? absX : -absX;
        visualRootToFlip.localScale = scale;
    }

    private void SetWalkAnimation(bool value)
    {
        if (animator != null && !string.IsNullOrEmpty(walkBoolName))
            animator.SetBool(walkBoolName, value);
    }
}
