using TMPro;
using UnityEditor;
using UnityEngine;

public class UpgradeWindow : MonoBehaviour
{
    private Room Room;

    [SerializeField] private TextMeshProUGUI RoomNameLabel;
    [SerializeField] private TextMeshProUGUI DescriptionLabel;
    [SerializeField] private TextMeshProUGUI CostLabel;

    [SerializeField] private TextMeshProUGUI CurrentLevel;
    [SerializeField] private TextMeshProUGUI CurrentCapacity;
    [SerializeField] private TextMeshProUGUI CurrentEfficency;
    [SerializeField] private TextMeshProUGUI CurrentEffects;

    [SerializeField] private TextMeshProUGUI NextLevel;
    [SerializeField] private TextMeshProUGUI NextCapacity;
    [SerializeField] private TextMeshProUGUI NextEfficency;
    [SerializeField] private TextMeshProUGUI NextEffects;

    public static UpgradeWindow Instance;
    public void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }
    public void OpenWindow(Room room)
    { 
        gameObject.SetActive(true);
        Room = room;
        UpdateLabels();
    }
    public void UpdateLabels()
    {
        if(Room == null) return;

        RoomNameLabel?.SetText(GetRoomName(Room.Type));
        DescriptionLabel?.SetText(Room.Description);
        CostLabel?.SetText($"{Room.UpgradeCost * (Room.Level + 1)}$");

        CurrentLevel?.SetText($"Уровень {Room.Level}");
        CurrentCapacity?.SetText($"{Room.Capacity}");
        CurrentEfficency?.SetText($"x{Room.Level}");

        NextLevel?.SetText($"Следующий уровень (Ур. {Room.Level})");
        NextCapacity?.SetText($"{Room.Capacity + 1}");
        NextEfficency?.SetText($"x{Room.Level + 1}");
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
    }
}
