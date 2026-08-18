using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public enum GamePhase
{
    Map,
    Preparation,
    Office,
    Report,
    Final
}

public enum PersonType
{
    Student,
    OfficeWorker
}
public enum CampaignType
{
    Reception,
    Donations,
    Propaganda,
    Laundry,
    Altar
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [HideInInspector] public District SelectedDistrict;
    

    [Header("Game")]
    public int Day = 1;
    [HideInInspector] public float Time;
    public GamePhase phase = GamePhase.Map;

    [Header("Resources")]
    public int Money = 600;
    public int Influence = 2;
    public int Grace = 0;
    public int Anxiety = 0;
    public float Hunger = 0;

    [SerializeField] private float HungerIncreaseSpeed = 0.2f;

    [Header("Limits")]
    public int MaxInfluence = 4;
    public float MaxHunger = 100;
    
    // =========================================================
    // PEOPLE
    // =========================================================

    [Header("Cultists")]
    public List<Person> reserve = new List<Person>();

    public List<Person> activeWorkers = new List<Person>();

    [SerializeField]
    private Transform startingCultistsParent;

    [SerializeField]
    private int startingCultistsCount = 3;

    // =========================================================
    // CITY
    // =========================================================

    [Header("City")]
    public int universityProgress;
    public int businessProgress;

    // =========================================================
    // KPI
    // =========================================================

    [Header("KPI")]
    public bool kpiStarted;

    public int kpiMoney;
    public int kpiContacts;

    // =========================================================
    // UI
    // =========================================================

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI MoneyLabel;
    [SerializeField] private TextMeshProUGUI DayLabel;
    [SerializeField] private TextMeshProUGUI TimeLabel;
    [SerializeField] private TextMeshProUGUI districtNameLabel;
    [SerializeField] private TextMeshProUGUI auditoryLabel;
    [SerializeField] private TextMeshProUGUI descriptionLabel;
    [SerializeField] private TextMeshProUGUI influenceLabel;
    [SerializeField] private Image[] timeSpeedButtons;
    [SerializeField] private Color selectedButtonColor = new Color(150, 240, 100);

    public GameObject StartWorkPanel;
    public GameObject TimeSpeedPanel;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        UpdateUI();
    }
    public void UpdateDistrictLabels()
    {
        if (SelectedDistrict != null)
        {
            districtNameLabel?.SetText(SelectedDistrict.Name);
            auditoryLabel?.SetText($"Аудитория: {SelectedDistrict.Auditory.ToLower()}");
            descriptionLabel?.SetText(SelectedDistrict.Description);
            influenceLabel?.SetText($"Влияние: {SelectedDistrict.Influence}/5");
        }
    }

    public void UpdateUI()
    {
        if(timeSpeedButtons.Length == 3)
        {
            foreach (var button in timeSpeedButtons) button.color = Color.white;

            switch(OfficeManager.Instance.timeSpeedModificator)
            {
                case 0:
                    timeSpeedButtons[0].color = selectedButtonColor;
                    break;
                case 1:
                    timeSpeedButtons[1].color = selectedButtonColor;
                    break;
                default:
                    timeSpeedButtons[2].color = selectedButtonColor;
                    break;
            }
        }
        
        MoneyLabel?.SetText($"{Money}$");
        DayLabel?.SetText($"День {Day}");
        TimeSpeedPanel?.SetActive(phase == GamePhase.Office);

        if (phase == GamePhase.Map)
        {
            TimeLabel?.SetText("Утро");
        }
        else TimeLabel?.SetText($"{(int)Time / 60:D2}:{(int)Time % 60:D2}");
    }


    // =========================================================
    // CLEAR PEOPLE
    // =========================================================

    private void ClearPeople()
    {
        HashSet<Person> peopleToDestroy =
            new HashSet<Person>();

        // Люди в резерве.
        foreach (Person person in reserve)
        {
            if (person != null)
            {
                peopleToDestroy.Add(person);
            }
        }

        // Люди, назначенные в комнаты.
        foreach (Person person in activeWorkers)
        {
            if (person != null)
            {
                peopleToDestroy.Add(person);
            }
        }

        foreach (Person person in peopleToDestroy)
        {
            Destroy(person.gameObject);
        }

        reserve.Clear();
        activeWorkers.Clear();
    }
    public bool SpendMoney(int amount)
    {
        if (amount < 0)
            return false;

        if (Money < amount)
            return false;

        Money -= amount;

        return true;
    }

    // =========================================================
    // INFLUENCE
    // =========================================================

    public bool SpendInfluence(int amount)
    {
        if (amount < 0)
            return false;

        if (Influence < amount)
            return false;

        Influence -= amount;

        return true;
    }
    public void AddGrace(int amount)
    {
        Grace += amount;

        if (Grace < 0)
        {
            Grace = 0;
        }
    }

    // =========================================================
    // ANXIETY
    // =========================================================

    public void AddAnxiety(int amount)
    {
        Anxiety = Mathf.Max(
            0,
            Anxiety + amount
        );
    }

    // =========================================================
    // HUNGER
    // =========================================================

    public void AddHunger(float amount)
    {
        Hunger = Mathf.Clamp(Hunger + amount * HungerIncreaseSpeed, 0, MaxHunger);
    }

    public void ReduceHunger(float amount)
    {
        Hunger = Mathf.Max(0, Hunger - amount);

        foreach(var person in activeWorkers)
        {
            if(!(person.Room is Laundry)) person.loyalty -= amount;
        }
    }
    public void AddPersonToReserve(Person person)
    {
        if (person == null) return;
        // Если человек уже находится в резерве, ничего не делаем.
        if (reserve.Contains(person)) return;
        // Максимум 8 человек.
        if (reserve.Count >= 8)
        {
            Debug.Log(
                $"[GameManager] Резерв заполнен. " +
                $"{person.name} удалён."
            );

            Destroy(person.gameObject);

            return;
        }
        reserve.Add(person);

        UpdateUI();
    }

    public void RemoveFromReserve(Person person)
    {
        if (person == null)
            return;

        reserve.Remove(person);

        UpdateUI();
    }
    public void EndDay()
    {
        phase = GamePhase.Report;

        UpdateUI();
    }

    public void NextDay()
    {
        if (Day >= 3)
        {
            phase = GamePhase.Final;

            UpdateUI();

            return;
        }
        Day++;
        activeWorkers.Clear();

        phase = GamePhase.Map;
        ScreenManager.Instance.OpenMenu(0);
    }
}