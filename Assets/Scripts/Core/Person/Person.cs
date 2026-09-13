using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public enum PersonType
{
    Student,
    OfficeWorker
}
[System.Serializable]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(DragPerson))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Button))]
public class Person : MonoBehaviour
{
    [HideInInspector] public PersonType Type;
    [HideInInspector] public float Loyalty;
    [HideInInspector] public Room Room;
    [HideInInspector] public RoomType RoomType;
    [HideInInspector] public float MaxLoyalty;
    [SerializeField] private float becomeCultistTime = 6f;
    [SerializeField] private float baseMaxLoyalty = 500f;
    [SerializeField] private float escapeSpeed = 1f;
    [SerializeField] private GameObject loyaltyPanel;
    [SerializeField] private TextMeshProUGUI loyaltyLabel;
    [SerializeField] private Slider loyaltyBar;
    [HideInInspector] public bool IsCultist = false;
    [HideInInspector] public bool IsEscaping = false;
    [HideInInspector] public bool HasElevated = false;
    [HideInInspector] public Image Image;
    public int MaxLaunderings = 3;
    private DragPerson dragPerson;
    private Transform elevator;
    private void Awake()
    {
        dragPerson = GetComponent<DragPerson>();
        Image = GetComponent<Image>();
        MaxLoyalty = baseMaxLoyalty;
        Loyalty = baseMaxLoyalty;
        elevator = FindFirstObjectByType<Elevator>().transform;
    }
    private void Update()
    {
        if (GameManager.Instance.Phase == GamePhase.Office)
        {
            dragPerson.enabled = IsCultist && !IsEscaping;
            
            if (Room != null)
            {
                RoomType = Room.Type;
            }
            else FindRoom();

            if (Room is Laundry || !IsCultist)
            {
                if (Loyalty < MaxLoyalty) Loyalty += Time.deltaTime * GameManager.Instance.GetTimeSpeed();
            }
            else Loyalty -= Time.deltaTime * GameManager.Instance.GetTimeSpeed();

            UpdateUI();

            if (Loyalty <= 0 && !IsEscaping) Escape();
        }
        if (IsEscaping) 
        {
            float direction;
            if (Room is Reception || HasElevated) direction = -1;
            else
            {
                direction = elevator.position.x - transform.position.x >= 0 ? 1 : -1;
            }  
            transform.localScale = new Vector2(direction, 1);
            transform.position += Vector3.right * direction * escapeSpeed * Time.deltaTime * GameManager.Instance.GetTimeSpeed();
        }       
    }
    private void UpdateUI()
    {
        

        loyaltyPanel?.SetActive(!IsEscaping && IsCultist);
        loyaltyLabel?.SetText($"ћракобесие: {GetLoyaltyPercent()}");
        loyaltyBar.value = Loyalty / MaxLoyalty;
    }
    public void Escape()
    {
        if (GameManager.Instance != null)
        {
            transform.SetParent(GameManager.Instance.OfficeCanvas);
            GameManager.Instance.AddEvidence(1);
        }
        IsEscaping = true;
    }
    public void Quit()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Reserve.Remove(this);
            GameManager.Instance.ActiveWorkers.Remove(this);
        }
        Destroy(gameObject);
    }
    public void BecomeCultist()
    {
        GameManager.Instance.ActiveWorkers.Add(this);
        GameManager.Instance.Reserve.Remove(this);
        StartCoroutine(StartRecruitment());
        IsCultist = true;
    }

    private IEnumerator StartRecruitment()
    {
        yield return new WaitForSeconds(becomeCultistTime / GameManager.Instance.GetTimeSpeed());
        if (IsCultist) Image.sprite = GameManager.Instance.CultistSprite;
    }

    public void Eat(float hungerReduction)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddEvidence(1);
            GameManager.Instance.ReduceHunger(hungerReduction);
            GameManager.Instance.Reserve.Remove(this);
            GameManager.Instance.ActiveWorkers.Remove(this);
        }
        Destroy(gameObject);
    }

    private int GetLoyaltyPercent()
    {
        return Mathf.RoundToInt(Loyalty / baseMaxLoyalty * 100f);
    }
    public void FindRoom()
    {
        switch (RoomType)
        {
            case RoomType.Reception:
                Room = FindAnyObjectByType<Reception>();
                break;
            case RoomType.Donations:
                Room = FindAnyObjectByType<DonationRoom>();
                break;
            case RoomType.Agitation:
                Room = FindAnyObjectByType<AgitationRoom>();
                break;
            case RoomType.Laundry:
                Room = FindAnyObjectByType<Laundry>();
                break;
            case RoomType.Altar:
                Room = FindAnyObjectByType<Altar>();
                break;
        }

        Room?.AssignPerson(this);
    }
}