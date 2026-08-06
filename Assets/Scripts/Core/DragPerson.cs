using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DragPerson : MonoBehaviour
{
    private Person person;
    private Vector3 offset;

    private void Awake()
    {
        person = GetComponent<Person>();
    }

    private void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + offset;
    }

    private void OnMouseUp()
    {
        Room room = FindRoomUnderPerson();

        if (room != null)
            room.Assign(person);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouse = Input.mousePosition;

        mouse.z = Mathf.Abs(Camera.main.transform.position.z);

        return Camera.main.ScreenToWorldPoint(mouse);
    }

    private Room FindRoomUnderPerson()
    {
        Collider2D[] hits =
            Physics2D.OverlapPointAll(transform.position);

        foreach (Collider2D hit in hits)
        {
            Room room = hit.GetComponent<Room>();

            if (room != null)
                return room;
        }

        return null;
    }
}
