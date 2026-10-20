using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


public class UpgradeWindow : MonoBehaviour
{
    private Room Room;

    [SerializeField] private TextMeshProUGUI RoomNameLabel;
    [SerializeField] private TextMeshProUGUI DescriptionLabel;
    [SerializeField] private TextMeshProUGUI CostLabel;
    [SerializeField] private Image RoomImage;

    [SerializeField] private TextMeshProUGUI CurrentLevel;
    [SerializeField] private TextMeshProUGUI CurrentCapacity;
    [SerializeField] private TextMeshProUGUI CurrentEfficency;
    [SerializeField] private TextMeshProUGUI CurrentEffects;

    [SerializeField] private GameObject NextLevelData;
    [SerializeField] private GameObject UpgradeButton;

    [SerializeField] private TextMeshProUGUI NextLevel;
    [SerializeField] private TextMeshProUGUI NextCapacity;
    [SerializeField] private TextMeshProUGUI NextEfficency;
    [SerializeField] private TextMeshProUGUI NextEffects;

    [SerializeField] private int TimeToClose = 5;

    public static UpgradeWindow Instance;
    private Coroutine closeWindowCoroutine;
    public void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }
    private IEnumerator CloseWindowCoroutine()
    {
        yield return new WaitForSeconds(TimeToClose);
        gameObject.SetActive(false);
    }
    public void OpenWindow(Room room)
    {        
        gameObject.SetActive(true);
        Room = room;
        UpdateUI();

        if(closeWindowCoroutine != null) 
        {
            StopCoroutine(closeWindowCoroutine);
        }
        closeWindowCoroutine = StartCoroutine(CloseWindowCoroutine());
    }
    public void UpdateUI()
    {
        if(Room == null) return;

        RoomNameLabel?.SetText(GetRoomName(Room.Type));     
        DescriptionLabel?.SetText(Room.Description);
        RoomImage.sprite = Room.Image.sprite;

        CurrentLevel?.SetText($"Уровень {Room.Level}");
        CurrentCapacity?.SetText($"{Room.Capacity}");
        CurrentEfficency?.SetText($"x{Room.Level}");
        CurrentEffects?.SetText(Room.GetBonusEffectDescription(Room.Level));
        int upgradeCost = Room.UpgradeCost * (Room.Level + 1);

        if (Room.Level < 3)
        {
            NextLevel?.SetText($"Следующий уровень (Ур. {Room.Level + 1})");
            NextCapacity?.SetText($"{Room.Capacity + 1}");
            NextEfficency?.SetText($"x{Room.Level + 1}");
            CurrentEffects?.SetText(Room.GetBonusEffectDescription(Room.Level + 1));
            CostLabel?.SetText($"{upgradeCost}$");
        }
        NextLevelData?.SetActive(Room.Level < 3);
        UpgradeButton?.SetActive(GameManager.Instance.Money >= upgradeCost);
    }
    private string GetRoomName(RoomType type)
    {
        switch (type)
        {
            case RoomType.Donations:
                return "Пожертвования";
            case RoomType.Agitation:
                return "Агитация";
            case RoomType.Altar:
                return "Алтарь";
            case RoomType.Laundry:
                return "Прачечная";
        }
        return "Приемная";
    }
    public void UpgradeRoom()
    {
        Room?.LevelUP();
        OpenWindow(Room);
    }
}
