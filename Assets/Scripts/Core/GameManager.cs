using System.Collections.Generic;
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

public enum RoomType
{
    Reception,
    Donations,
    Agitation,
    Laundry,
    Altar
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

    // =========================================================
    // RESOURCES
    // =========================================================

    [Header("Resources")]
    public int Money = 600;
    public int Influence = 2;
    public int Grace = 0;
    public int Anxiety = 0;
    public float Hunger = 0;

    [Header("Limits")]
    public int maxInfluence = 4;
    public int maxHunger = 100;
    
    // =========================================================
    // PEOPLE
    // =========================================================

    [Header("Cultists")]
    public List<Person> reserve = new List<Person>();

    public List<Person> activeWorkers =
        new List<Person>();

    [Header("Starting Cultists")]
    [SerializeField]
    private Person personPrefab;

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

    private void Start()
    {
        StartNewGame();
    }

    private void Update()
    {
        UpdateLabels();
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

    public void UpdateLabels()
    {
        MoneyLabel?.SetText($"{Money}$");
        DayLabel?.SetText($"День {Day}");
        TimeSpeedPanel?.SetActive(phase == GamePhase.Office);

        if (phase == GamePhase.Map)
        {
            TimeLabel?.SetText("Утро");
        }
        else TimeLabel?.SetText($"{(int)Time / 60:D2}:{(int)Time % 60:D2}");
    }

    public void StartNewGame()
    {
        Day = 1;
        phase = GamePhase.Map;
        StartWorkPanel?.SetActive(false);


        Money = 600;
        Influence = 2;
        Grace = 0;
        Anxiety = 0;
        Hunger = 0;

        ClearPeople();

        // -----------------------------------------------------
        // Город
        // -----------------------------------------------------

        universityProgress = 0;
        businessProgress = 0;

        // -----------------------------------------------------
        // KPI
        // -----------------------------------------------------

        kpiStarted = false;
        kpiMoney = 0;
        kpiContacts = 0;

        UpdateLabels();
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
        Hunger = Mathf.Clamp(Hunger + amount, 0, maxHunger);
    }

    public void ReduceHunger(int amount)
    {
        Hunger = Mathf.Max(0, Hunger - amount);
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

        UpdateLabels();
    }

    public void RemoveFromReserve(Person person)
    {
        if (person == null)
            return;

        reserve.Remove(person);

        UpdateLabels();
    }
    public void EndDay()
    {
        phase = GamePhase.Report;

        UpdateLabels();
    }

    public void NextDay()
    {
        if (Day >= 3)
        {
            phase = GamePhase.Final;

            UpdateLabels();

            return;
        }
        Day++;
        activeWorkers.Clear();
        // Снижаем подозрение культистов.
        foreach (Person person in reserve)
        {
            if (person == null) continue;
            person.Suspicion = Mathf.Max(0, person.Suspicion - 1);
        }
        phase = GamePhase.Map;
        ScreenManager.Instance.OpenMenu(0);
    }
}