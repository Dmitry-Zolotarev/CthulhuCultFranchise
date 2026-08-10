using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
public class Room : MonoBehaviour, IDropHandler
{
    [Header("Room")]
    public RoomType roomType;

    [Header("Capacity")]
    public int capacity = 1;

    [Header("Level")]
    public int Level = 1;

    [SerializeField] private int maxLevel = 3;
    [SerializeField] protected Sprite[] LevelSprites;

    [Header("People Layout")]
    [SerializeField] private float personSpacing = 100f;

    [SerializeField] private float personOffsetY = 0f;

    private Image roomImage;

    // =========================================================
    // UNITY
    // =========================================================

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
        if (eventData.pointerDrag == null)
            return;

        DragPerson dragPerson =
            eventData.pointerDrag.GetComponent<DragPerson>();

        if (dragPerson == null)
            return;

        Person person =
            eventData.pointerDrag.GetComponent<Person>();

        if (person == null)
        {
            Debug.LogWarning(
                $"Room {roomType}: у перетаскиваемого объекта " +
                $"нет компонента Person."
            );

            return;
        }

        if (!CanAcceptPerson(person))
        {
            Debug.Log(
                $"Комната {roomType} заполнена."
            );

            return;
        }

        AssignPerson(
            person,
            dragPerson
        );
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

    private void AssignPerson(
        Person person,
        DragPerson dragPerson)
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

        person.currentRoom =
            roomType;

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

        dragPerson.SetDropped();

        Debug.Log(
            $"{person.name} назначен в комнату {roomType}."
        );
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
            Person person =
                transform
                    .GetChild(i)
                    .GetComponent<Person>();

            if (person == null)
                continue;

            RectTransform rect =
                person.GetComponent<RectTransform>();

            if (rect != null)
            {
                people.Add(rect);
            }
        }

        int count = people.Count;

        if (count == 0)
            return;

        // -----------------------------------------------------
        // Вычисляем общую ширину ряда.
        //
        // Например:
        // 1 человек → 0
        // 2 человека → spacing
        // 3 человека → spacing * 2
        // -----------------------------------------------------

        float totalWidth =
            (count - 1) * personSpacing;

        // Начинаем слева от центра.
        float startX =
            -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            RectTransform person =
                people[i];

            float x =
                startX +
                i * personSpacing;

            person.anchoredPosition =
                new Vector2(
                    x,
                    personOffsetY
                );

            person.localRotation =
                Quaternion.identity;

            person.localScale =
                Vector3.one;
        }
    }

    // =========================================================
    // LEVEL UP
    // =========================================================

    public void LevelUP()
    {
        if (Level >= maxLevel)
        {
            Debug.Log(
                $"Комната {roomType} уже имеет максимальный уровень."
            );

            return;
        }

        Level++;

        UpdateRoomSprite();

        Debug.Log(
            $"Комната {roomType} улучшена до уровня {Level}."
        );
    }

    // =========================================================
    // ROOM SPRITE
    // =========================================================

    private void UpdateRoomSprite()
    {
        if (roomImage == null)
            return;

        if (LevelSprites == null ||
            LevelSprites.Length == 0)
        {
            return;
        }

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