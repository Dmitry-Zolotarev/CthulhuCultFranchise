using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class DragPerson : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private Transform originalParent;
    private Vector2 originalPosition;

    private bool wasDropped;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError(
                $"DragPerson: Canvas не найден дл€ {name}."
            );
        }
    }

    // =========================================================
    // BEGIN DRAG
    // =========================================================

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvas == null)
            return;

        wasDropped = false;

        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;

        // ѕока тащим человека, он не должен
        // блокировать Room от получени€ Drop.
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.75f;

        // ѕоднимаем человека на верхний уровень Canvas.
        transform.SetParent(
            canvas.transform,
            true
        );

        Debug.Log(
            $"Ќачато перемещение {name}."
        );
    }

    // =========================================================
    // DRAG
    // =========================================================

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null)
            return;

        RectTransform canvasRect =
            canvas.transform as RectTransform;

        if (canvasRect == null)
            return;

        Camera eventCamera =
            canvas.renderMode ==
            RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;

        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                eventData.position,
                eventCamera,
                out localPoint))
        {
            rectTransform.localPosition =
                localPoint;
        }
    }

    // =========================================================
    // END DRAG
    // =========================================================

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // Room.OnDrop() устанавливает этот флаг,
        // если человек успешно прин€т.
        if (wasDropped)
            return;

        ReturnToOriginalPosition();
    }

    // =========================================================
    // DROP RESULT
    // =========================================================

    public void SetDropped()
    {
        wasDropped = true;
    }

    // =========================================================
    // RETURN
    // =========================================================

    private void ReturnToOriginalPosition()
    {
        if (originalParent == null)
            return;

        transform.SetParent(
            originalParent,
            false
        );

        rectTransform.anchoredPosition =
            originalPosition;

        rectTransform.localRotation =
            Quaternion.identity;

        rectTransform.localScale =
            Vector3.one;
    }
}