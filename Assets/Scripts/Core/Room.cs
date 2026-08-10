using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

        int currentCount = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            Person existingPerson =
                transform
                    .GetChild(i)
                    .GetComponent<Person>();

            if (existingPerson == null)
                continue;

            // Самого себя не считаем.
            if (existingPerson == person)
                continue;

            currentCount++;
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
        // Убираем из приёмной / резерва.
        // -----------------------------------------------------

        GameManager.Instance.RemoveFromReserve(
            person
        );

        // -----------------------------------------------------
        // Добавляем в активных работников.
        // -----------------------------------------------------

        if (!GameManager.Instance.activeWorkers.Contains(person))
        {
            GameManager.Instance.activeWorkers.Add(
                person
            );
        }

        // -----------------------------------------------------
        // Записываем текущую комнату.
        // -----------------------------------------------------

        person.currentRoom =
            roomType;

        // -----------------------------------------------------
        // Перемещаем Person внутрь комнаты.
        // -----------------------------------------------------

        person.transform.SetParent(
            transform,
            false
        );

        RectTransform rect =
            person.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchoredPosition =
                Vector2.zero;

            rect.localPosition =
                Vector3.zero;

            rect.localRotation =
                Quaternion.identity;

            rect.localScale =
                Vector3.one;
        }

        // -----------------------------------------------------
        // Сообщаем DragPerson, что Drop успешный.
        // -----------------------------------------------------

        dragPerson.SetDropped();

        Debug.Log(
            $"{person.name} назначен в комнату {roomType}."
        );
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

        // Level 1 → индекс 0
        // Level 2 → индекс 1
        // Level 3 → индекс 2

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
            if (transform
                    .GetChild(i)
                    .GetComponent<Person>() != null)
            {
                count++;
            }
        }

        return count;
    }

    public bool IsFull()
    {
        return GetCurrentPersonCount() >= capacity;
    }
}