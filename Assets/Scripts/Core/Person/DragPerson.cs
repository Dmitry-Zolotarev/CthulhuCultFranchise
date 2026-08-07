using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class DragPerson :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Transform originalParent;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform =
            GetComponent<RectTransform>();

        canvas =
            GetComponentInParent<Canvas>();

        canvasGroup =
            GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(
        PointerEventData eventData)
    {
        if (canvas == null)
            return;

        originalParent =
            transform.parent;

        originalPosition =
            rectTransform.anchoredPosition;

        // Перемещаем человека наверх Canvas,
        // чтобы он не оказался под другими UI-элементами.
        transform.SetParent(
            canvas.transform,
            true
        );

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(
        PointerEventData eventData)
    {
        if (canvas == null)
            return;

        RectTransform canvasRect =
            canvas.GetComponent<RectTransform>();

        Vector2 localPosition;

        Camera eventCamera =
            eventData.pressEventCamera;

        bool success =
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                eventData.position,
                eventCamera,
                out localPosition
            );

        if (success)
        {
            rectTransform.localPosition =
                localPosition;
        }
    }

    public void OnEndDrag(
        PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        Room room =
            FindRoomUnderPointer(eventData);

        if (room != null)
        {
            PlaceIntoRoom(room);
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    private void PlaceIntoRoom(Room room)
    {
        Person person =
            GetComponent<Person>();

        if (person == null)
        {
            Debug.LogError(
                "На Person отсутствует Person.cs!"
            );

            ReturnToOriginalPosition();
            return;
        }

        // Передаём человека комнате.
        room.Assign(person);

        // После успешного назначения
        // помещаем UI-объект внутрь комнаты.
        transform.SetParent(
            room.transform,
            false
        );

        rectTransform.anchoredPosition =
            Vector2.zero;
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent, false);
        rectTransform.anchoredPosition = originalPosition;

    }

    private Room FindRoomUnderPointer(
        PointerEventData eventData)
    {
        if (EventSystem.current == null)
            return null;

        // Получаем все UI-элементы под мышью.
        var results =
            new System.Collections.Generic.List<RaycastResult>();

        EventSystem.current.RaycastAll(
            eventData,
            results
        );

        foreach (RaycastResult result in results)
        {
            Room room =
                result.gameObject.GetComponentInParent<Room>();

            if (room != null)
                return room;
        }

        return null;
    }
}