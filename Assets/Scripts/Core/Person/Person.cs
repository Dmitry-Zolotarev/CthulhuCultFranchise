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
public class Person : MonoBehaviour
{
    [HideInInspector] public PersonType Type;
    [HideInInspector] public float Loyalty;
    [HideInInspector] public Room Room;
    [HideInInspector] public RoomType RoomType;
    [HideInInspector] public float MaxLoyalty;
    [SerializeField] private float becomeCultistTime = 6f;
    [SerializeField] private float baseMaxLoyalty = 500f;
    [SerializeField] private GameObject loyaltyPanel;
    [SerializeField] private TextMeshProUGUI loyaltyLabel;
    [SerializeField] private Slider loyaltyBar;
    [HideInInspector] public Image Image;
    [HideInInspector] public bool IsCultist = false;

    private void Awake()
    {
        Image = GetComponent<Image>();
        MaxLoyalty = baseMaxLoyalty;
        Loyalty = baseMaxLoyalty;
    }
    private void Update()
    {
        if (GameManager.Instance.Phase == GamePhase.Office)
        {
            if (Room != null)
            {
                RoomType = Room.Type;
            }
            else FindRoom();

            if(Image.sprite == GameManager.Instance.CultistSprite && (Room is Reception))
            {
                GameManager.Instance.ActiveWorkers.Remove(this);
                GameManager.Instance.Reserve.Remove(this);
                Destroy(gameObject);
                return;
            }

            if (Room is Laundry || !IsCultist)
            {
                if (Loyalty < MaxLoyalty) Loyalty += Time.deltaTime * GameManager.Instance.GetTimeSpeed();
            }
            else Loyalty -= Time.deltaTime * GameManager.Instance.GetTimeSpeed();

            UpdateUI();

            if (Loyalty <= 0) Escape();
        }
    }

    private void UpdateUI()
    {
        loyaltyPanel?.SetActive(IsCultist && !(Room is Reception) && !(Room is Altar));
        loyaltyLabel?.SetText($"ћракобесие: {GetLoyaltyPercent()}");
        loyaltyBar.value = Loyalty / MaxLoyalty;
    }

    public void Escape()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Reserve.Remove(this);
            GameManager.Instance.ActiveWorkers.Remove(this);
            GameManager.Instance.AddAnxiety(1);
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
            GameManager.Instance.AddAnxiety(1);
            GameManager.Instance.ReduceHunger(hungerReduction);
            GameManager.Instance.Reserve.Remove(this);
            GameManager.Instance.ActiveWorkers.Remove(this);
        }
        Destroy(gameObject);
    }

    private int GetLoyaltyPercent()
    {
        return Mathf.RoundToInt(Loyalty / MaxLoyalty * 100f);
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