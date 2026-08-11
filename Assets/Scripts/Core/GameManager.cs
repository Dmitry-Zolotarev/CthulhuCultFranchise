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
    Propaganda,
    Laundry,
    Altar
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // =========================================================
    // GAME STATE
    // =========================================================

    [Header("Game")]
    public int day = 1;
    public GamePhase phase = GamePhase.Map;

    // =========================================================
    // RESOURCES
    // =========================================================

    [Header("Resources")]
    public int Money = 600;
    public int Influence = 2;
    public int Grace = 0;
    public int Anxiety = 0;
    public int Hunger = 0;

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
    [SerializeField]
    private TextMeshProUGUI MoneyLabel;

    [SerializeField]
    private TextMeshProUGUI GraceLabel;

    [SerializeField]
    private TextMeshProUGUI InfuenceLabel;

    [SerializeField]
    private TextMeshProUGUI AnxietyLabel;

    [SerializeField]
    private TextMeshProUGUI HungerLabel;

    [SerializeField]
    private TextMeshProUGUI CultistCountLabel;

    // =========================================================
    // UNITY
    // =========================================================

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
        UpdateUI();
    }

    // =========================================================
    // UI
    // =========================================================

    private void UpdateUI()
    {
        MoneyLabel?.SetText($"{Money}");
        GraceLabel?.SetText($"{Grace}");
        InfuenceLabel?.SetText($"{Influence}");
        AnxietyLabel?.SetText($"{Anxiety}");
        HungerLabel?.SetText($"{Hunger}");
        CultistCountLabel?.SetText($"{reserve.Count + activeWorkers.Count}");
    }

    // =========================================================
    // NEW GAME
    // =========================================================

    public void StartNewGame()
    {
        day = 1;

        phase = GamePhase.Map;

        Money = 600;
        Influence = 2;
        Grace = 0;
        Anxiety = 0;
        Hunger = 0;

        // -----------------------------------------------------
        // Очищаем старых людей
        // -----------------------------------------------------

        ClearPeople();

        // -----------------------------------------------------
        // Создаём стартовых культистов
        // -----------------------------------------------------

        SpawnStartingCultists();

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

        UpdateUI();
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

    // =========================================================
    // STARTING CULTISTS
    // =========================================================

    private void SpawnStartingCultists()
    {
        if (personPrefab == null)
        {
            Debug.LogError(
                "[GameManager] Person Prefab не назначен!"
            );

            return;
        }

        if (startingCultistsParent == null)
        {
            Debug.LogError(
                "[GameManager] Starting Cultists Parent " +
                "не назначен!"
            );

            return;
        }

        for (int i = 0; i < startingCultistsCount; i++)
        {
            Person person = Instantiate(
                personPrefab,
                startingCultistsParent,
                false
            );

            if (person == null)
            {
                Debug.LogError(
                    "[GameManager] Не удалось создать Person."
                );

                continue;
            }

            // -------------------------------------------------
            // Основные данные
            // -------------------------------------------------

            person.currentRoom =
                RoomType.Reception;

            person.type =
                i % 2 == 0
                    ? PersonType.Student
                    : PersonType.OfficeWorker;

            person.loyalty = 1;
            person.contacts = 1;
            person.Suspicion = 0;

            // -------------------------------------------------
            // Позиция UI
            // -------------------------------------------------

            RectTransform rect =
                person.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.anchoredPosition =
                    new Vector2(
                        i * 100f,
                        0f
                    );

                rect.localRotation =
                    Quaternion.identity;

                rect.localScale =
                    Vector3.one;
            }

            // -------------------------------------------------
            // Добавляем в резерв
            // -------------------------------------------------

            if (!reserve.Contains(person))
            {
                reserve.Add(person);
            }

            Debug.Log(
                $"[GameManager] Создан культист " +
                $"{i + 1}/{startingCultistsCount}. " +
                $"Reserve = {reserve.Count}"
            );
        }

        Debug.Log(
            $"[GameManager] Стартовых культистов создано: " +
            $"{reserve.Count}"
        );
    }

    // =========================================================
    // CULTIST COUNT
    // =========================================================

    // =========================================================
    // MONEY
    // =========================================================

    public bool SpendMoney(int amount)
    {
        if (amount < 0)
            return false;

        if (Money < amount)
            return false;

        Money -= amount;

        return true;
    }

    public void AddMoney(int amount)
    {
        Money += amount;

        if (Money < 0)
        {
            Money = 0;
        }

        if (kpiStarted && amount > 0)
        {
            kpiMoney += amount;
        }
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

    public void AddInfluence(int amount)
    {
        Influence = Mathf.Clamp(
            Influence + amount,
            0,
            maxInfluence
        );
    }

    // =========================================================
    // GRACE
    // =========================================================

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

    public void AddHunger(int amount)
    {
        Hunger = Mathf.Clamp(
            Hunger + amount,
            0,
            maxHunger
        );
    }

    public void ReduceHunger(int amount)
    {
        Hunger = Mathf.Max(
            0,
            Hunger - amount
        );
    }

    // =========================================================
    // KPI
    // =========================================================

    public void StartKPI()
    {
        kpiStarted = true;

        kpiMoney = 0;
        kpiContacts = 0;
    }

    public void RegisterContactConverted()
    {
        if (!kpiStarted)
            return;

        kpiContacts++;
    }

    public bool IsKPIComplete()
    {
        return
            kpiMoney >= 400 &&
            kpiContacts >= 2 &&
            Hunger < 75;
    }

    // =========================================================
    // PEOPLE / RESERVE
    // =========================================================

    public void AddPersonToReserve(Person person)
    {
        if (person == null)
            return;

        // Если человек уже находится в резерве,
        // ничего не делаем.
        if (reserve.Contains(person))
            return;

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

    // =========================================================
    // DAY
    // =========================================================

    public void EndDay()
    {
        phase = GamePhase.Report;

        UpdateUI();
    }

    public void NextDay()
    {
        if (day >= 3)
        {
            phase = GamePhase.Final;

            UpdateUI();

            return;
        }

        day++;

        // +2 влияния в начале нового дня.
        AddInfluence(2);

        activeWorkers.Clear();

        // Снижаем подозрение культистов.
        foreach (Person person in reserve)
        {
            if (person == null)
                continue;

            person.Suspicion =
                Mathf.Max(
                    0,
                    person.Suspicion - 1
                );
        }

        phase = GamePhase.Map;

        UpdateUI();
    }
}