using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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
    public int capacity = 1;
    public int Level = 1;
    [SerializeField] private int maxLevel = 3;
    [SerializeField] protected Sprite[] LevelSprites;
    [SerializeField] private float personSpacing = 100f;
    [SerializeField] private float personOffsetY = -30f;
    [HideInInspector] public RoomType Type = RoomType.Reception;
    private Image roomImage;
    
    private void Awake()
    {
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
        roomImage = GetComponent<Image>();
        UpdateRoomSprite();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        Person person = eventData.pointerDrag.GetComponent<Person>();
        if (person == null) return;
        if (!CanAcceptPerson(person)) return;
        AssignPerson(person);
    }
    private bool CanAcceptPerson(Person person)
    {
        if (person == null || this is Reception) return false;
        int currentCount = GetCurrentPersonCount();
        if (person.transform.parent == transform) currentCount--;
        return currentCount < capacity;
    }
    public virtual void AssignPerson(Person person)
    {
        if (IsFull()) return;
        
        if (!(this is Reception) && !(this is Altar))
        {
            person.BecomeCultist();        
        }
        else if(GetCurrentPersonCount() >= capacity)
        {
            Destroy(person.gameObject);
            return;
        }
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
        SetLevel(Level + 1);
    }
    private void UpdateRoomSprite()
    {
        if (roomImage == null || LevelSprites == null || LevelSprites.Length == 0) return;

        int spriteIndex = Mathf.Clamp(Level - 1, 0, LevelSprites.Length - 1);

        if (LevelSprites[spriteIndex] != null) roomImage.sprite = LevelSprites[spriteIndex];

    }

    public int GetCurrentPersonCount()
    {
        int count = 0;

        for (int i = 0; i < transform.childCount; i++)
        {        
            Person person = transform.GetChild(i).GetComponent<Person>();

            if (person != null) count++;
            if (this is Reception && count > capacity) Destroy(person);
        }
        return count;
    }

    public bool IsFull()
    {
        return GetCurrentPersonCount() >= capacity;
    }
}