using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
public class Room : MonoBehaviour, IDropHandler
{
    public RoomType roomType = RoomType.Reception;
    public int capacity = 1;

    public int Level = 1;

    [SerializeField] private int maxLevel = 3;
    [SerializeField] protected Sprite[] LevelSprites;

    [SerializeField] private float personSpacing = 100f;

    [SerializeField] private float personOffsetY = 0f;

    private Image roomImage;
    
    private void Awake()
    {
        roomImage = GetComponent<Image>();

        UpdateRoomSprite();
    }

    // =========================================================
    // DROP
    // =========================================================

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        Person person = eventData.pointerDrag.GetComponent<Person>();
        if (person == null) return;
        if (!CanAcceptPerson(person)) return;

        AssignPerson(person);

    }

    // =========================================================
    // CAPACITY
    // =========================================================

    private bool CanAcceptPerson(Person person)
    {
        if (person == null)
            return false;

        int currentCount = GetCurrentPersonCount();

        // Если человек уже находится в этой комнате,
        // не считаем его второй раз.
        if (person.transform.parent == transform)
        {
            currentCount--;
        }

        return currentCount < capacity;
    }

    // =========================================================
    // ASSIGN PERSON
    // =========================================================

    public void AssignPerson(Person person)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "Room: GameManager.Instance не найден."
            );

            return;
        }

        // -----------------------------------------------------
        // Убираем из резерва
        // -----------------------------------------------------

        GameManager.Instance.RemoveFromReserve(
            person
        );

        // -----------------------------------------------------
        // Добавляем в активных работников
        // -----------------------------------------------------

        if (!GameManager.Instance.activeWorkers.Contains(person))
        {
            GameManager.Instance.activeWorkers.Add(
                person
            );
        }

        // -----------------------------------------------------
        // Записываем комнату
        // -----------------------------------------------------

        person.currentRoom = roomType;


        // -----------------------------------------------------
        // Перемещаем человека в комнату
        // -----------------------------------------------------

        person.transform.SetParent(
            transform,
            false
        );

        RectTransform rect =
            person.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.localRotation =
                Quaternion.identity;

            rect.localScale =
                Vector3.one;
        }

        // -----------------------------------------------------
        // Выстраиваем всех людей в комнате
        // -----------------------------------------------------

        ArrangePeople();

        // -----------------------------------------------------
        // Сообщаем DragPerson об успешном Drop
        // -----------------------------------------------------
        person?.GetComponent<DragPerson>()?.SetDropped();
    }

    // =========================================================
    // ARRANGE PEOPLE
    // =========================================================

    private void ArrangePeople()
    {
        List<RectTransform> people =
            new List<RectTransform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Person person = transform.GetChild(i).GetComponent<Person>();
            if (person != null)
            {
                RectTransform rect = person.GetComponent<RectTransform>();
                if (rect != null) people.Add(rect);
            }            
        }

        int count = people.Count;

        if (count == 0)
            return;
        float totalWidth =
            (count - 1) * personSpacing;

        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            RectTransform person = people[i];
            float x = startX + i * personSpacing;
            person.anchoredPosition = new Vector2(x, personOffsetY);
            person.localRotation = Quaternion.identity;
            person.localScale = Vector3.one;

        }
    }

    // =========================================================
    // LEVEL UP
    // =========================================================

    public void LevelUP()
    {
        if (Level < maxLevel)
        {
            Level++;
            UpdateRoomSprite();
        }             
    }

    private void UpdateRoomSprite()
    {
        if (roomImage == null) return;


        if (LevelSprites == null || LevelSprites.Length == 0) return;

        int spriteIndex =
            Mathf.Clamp(
                Level - 1,
                0,
                LevelSprites.Length - 1
            );

        if (LevelSprites[spriteIndex] != null)
        {
            roomImage.sprite =
                LevelSprites[spriteIndex];
        }
    }

    // =========================================================
    // INFO
    // =========================================================

    public int GetCurrentPersonCount()
    {
        int count = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            Person person = transform.GetChild(i).GetComponent<Person>();
            if (person != null) count++;
        }

        return count;
    }

    public bool IsFull()
    {
        return GetCurrentPersonCount() >= capacity;
    }
}