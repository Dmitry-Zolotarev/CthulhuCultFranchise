using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
public enum RoomType
{
    Reception,
    Donations,
    Agitation,
    Laundry,
    Altar
}
[RequireComponent(typeof(Image))]
public class Room : MonoBehaviour, IDropHandler
{
    public int Capacity = 1;
    [Range(1, 3)] public int Level = 1;
    [Range(1, 3)] public int Floor = 1;
    [SerializeField] private int maxLevel = 3;
    [SerializeField] protected Sprite[] LevelSprites;
    [SerializeField] private float personSpacing = 100f;
    [SerializeField] private float personOffsetY = -30f;
    [HideInInspector] public RoomType Type = RoomType.Reception;
    [SerializeField] private TextMeshProUGUI roomNameLabel;
    [SerializeField] private TextMeshProUGUI capacityLabel;
    public string Description = "";
    public int UpgradeCost = 500;

    private string roomName;
    [HideInInspector] public Image Image;
    private void Awake()
    {
        roomName = roomNameLabel.text;
        Image = GetComponent<Image>();
        if (this is DonationRoom)
        {
            Type = RoomType.Donations;
        } 
        else if (this is AgitationRoom)
        {
            Type = RoomType.Agitation;
        }
        if (this is Laundry)
        {
            Type = RoomType.Laundry;
        }
        if (this is Altar)
        {
            Type = RoomType.Altar;
        }       
        UpdateRoomSprite();
    }
    protected void Update()
    {
        roomNameLabel?.SetText(roomName + " ур. " + Level);
        capacityLabel?.SetText($"{GetCurrentPersonCount()}/{Capacity}");
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        Person person = eventData.pointerDrag.GetComponent<Person>();
        if (person == null) return;
        AssignPerson(person);
    }
    public virtual void AssignPerson(Person person)
    {
        if (IsFull() && person.Room != this) return;
        person.Room = this;
        person.transform.SetParent(transform, false);
        RectTransform rect = person.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localRotation =
                Quaternion.identity;

            rect.localScale =
                Vector3.one;
        }
        ArrangePeople();
        person?.GetComponent<DragPerson>()?.SetDropped();
    }

    private void ArrangePeople()
    {
        List<RectTransform> people = new List<RectTransform>();


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

        if (count == 0) return;

        float totalWidth = (count - 1) * personSpacing;

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
    public void SetLevel(int lvl)
    {
        Level = lvl;
        UpdateRoomSprite();
    }
    public void LevelUP()
    {
        if (GameManager.Instance.TrySpendMoney(GetUpdateCost()))
        {
            SetLevel(Level + 1);
        } 
    }
    public void OpenUpgradeWindow()
    {
        UpgradeWindow.Instance.OpenWindow(this);
    }
    private int GetUpdateCost()
    {
        return UpgradeCost * (int)Mathf.Pow(2, Level);
    }
    private void UpdateRoomSprite()
    {
        if (Image == null || LevelSprites == null || LevelSprites.Length == 0) return;

        int spriteIndex = Mathf.Clamp(Level - 1, 0, LevelSprites.Length - 1);

        if (LevelSprites[spriteIndex] != null) Image.sprite = LevelSprites[spriteIndex];

    }

    public int GetCurrentPersonCount()
    {
        int count = 0;
        var people = FindObjectsOfType<Person>();
        foreach (var person in people)
        {
            if (person.RoomType == Type) count++;
        }
        return count;
    }

    public bool IsFull()
    {
        return GetCurrentPersonCount() >= Capacity;
    }
}