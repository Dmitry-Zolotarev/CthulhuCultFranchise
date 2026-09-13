using System.Collections.Generic;
using System.Collections;
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
    public static GameManager Instance; 
    public int Day = 1;
   
    [HideInInspector] public float DayTime;
    public GamePhase Phase = GamePhase.Map;

    [Header("Resources")]
    public int Money = 100;
    [HideInInspector] public int TotalEarned = 0;
    [HideInInspector] public int TodayEarned = 0;
    [HideInInspector] public int TodayInfluence = 0;
    [HideInInspector] public int TodayEvidences = 0;


    [HideInInspector] public int EvidencesCount = 0;
    [HideInInspector] public float Hunger = 0;
    [HideInInspector] public District SelectedDistrict;   
    [HideInInspector] public HashSet<Person> Reserve = new HashSet<Person>();
    [HideInInspector] public HashSet<Person> ActiveWorkers = new HashSet<Person>();

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI moneyLabel;
    
    [SerializeField] private TextMeshProUGUI dayLabel;
    [SerializeField] private TextMeshProUGUI timeLabel ;
    [SerializeField] private TextMeshProUGUI districtNameLabel;
    [SerializeField] private TextMeshProUGUI auditoryLabel;
    [SerializeField] private TextMeshProUGUI evidencesCountLabel;
    [SerializeField] private TextMeshProUGUI descriptionLabel;
    [SerializeField] private TextMeshProUGUI influenceLabel;
    [SerializeField] private TextMeshProUGUI todayEarnLabel;
    [SerializeField] private TextMeshProUGUI todayTaxLabel;
    [SerializeField] private Image[] timeSpeedButtons;
    [SerializeField] private Color selectedButtonColor = new Color(150, 240, 100);
    [SerializeField] private TextMeshProUGUI hungerPercentLabel;
    [SerializeField] private GameObject TimeSpeedPanel;
    [SerializeField] private Slider hungerBar;
    [SerializeField] private Slider anxietyBar;
    public GameObject StartWorkPanel;

    [Header("Prefabs")]
    public GameObject[] Canvases;
    public Sprite CultistSprite;
    public Sprite[] PersonSprites;
    public Transform OfficeCanvas;
    public Room[] Rooms;

    [SerializeField] private Person personPrefab;
    [SerializeField] private Reception reception;
    

    [Header("Balance settings")]
    public float MaxHunger = 100;
    
    [SerializeField] private float HungerIncreaseSpeed = 0.2f;
    [SerializeField] private float MaxAnxiety = 20;
    [SerializeField] private float visitInterval = 15f;
    [SerializeField] private float startTime = 600f;
    [SerializeField] private float endTime = 1080f;
    [SerializeField] private float timeSpeed = 6f;
    [SerializeField] private int visitorsCount = 6;
    public float hungerReduction = 50f;
    [Header("Audio")]
    [SerializeField] private AudioClip officeMusic;
    [HideInInspector] public float AgitationProgress = 0;
    [HideInInspector] public int TimeSpeedModificator = 1;
    [HideInInspector] public District[] Districts;
    private Coroutine spawnCoroutine;
    private void Awake()
    {
        OpenCanvas(0);
        Instance = this;
        StartWorkPanel.SetActive(false);
        Districts = FindObjectsOfType<District>();
        if (SaveManager.NeedLoad) SaveManager.Load();
    }
    public void StartShift()
    {
        TodayEarned = 0;
        DayTime = startTime;
        Phase = GamePhase.Office;
        StartWorkPanel.SetActive(false);

        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnVisitors());
        
    }
    
    private void Update()
    {
        if (Phase == GamePhase.Office) 
        {
            if (spawnCoroutine == null) spawnCoroutine = StartCoroutine(SpawnVisitors());
            OpenCanvas(1);
            MusicPlayer.Instance?.PlayMusic(officeMusic);
            DayTime += Time.deltaTime * timeSpeed * TimeSpeedModificator;
            if (DayTime >= endTime) FinishShift();

            Hunger += GetTimeSpeed() * Time.deltaTime * HungerIncreaseSpeed;
            if (Hunger > MaxHunger) TriggerCthulhuEating();        
        }  
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
        foreach (var button in timeSpeedButtons) button.color = Color.white;
        switch (TimeSpeedModificator)
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
        moneyLabel?.SetText($"{Money}$");
        evidencesCountLabel?.SetText($"Найдено улик: {EvidencesCount}");
        todayEarnLabel?.SetText($"Доход за прошлую смену: {TodayEarned}$");
        todayTaxLabel?.SetText($"{TaxManager.Instance.TodayTax}$");
        dayLabel?.SetText($"День {Day}");
        TimeSpeedPanel?.SetActive(Phase == GamePhase.Office);

        hungerBar.value = Hunger / MaxHunger;
        anxietyBar.value = EvidencesCount / MaxAnxiety;
        hungerPercentLabel?.SetText($"{(int)(hungerBar.value * 100)}%");  

        if (Phase == GamePhase.Map)
        {
            timeLabel?.SetText("Утро");
        }
        else timeLabel?.SetText($"{(int)DayTime / 60:D2}:{(int)DayTime % 60:D2}");
    }
    public void AddMoney(int amount)
    {
        if (amount > 0) 
        {
            Money += amount;
            TodayEarned += amount;
            TotalEarned += amount;
            TaxManager.Instance.AddTax(amount);
        }         
    }
    public void AddEvidence(int amount)
    {
        if(amount > 0)
        {
            EvidencesCount += amount;
            TodayEvidences += amount;
        }        
    }
    public void AddInfluence(int amount)
    {
        SelectedDistrict.Influence++;
        TodayInfluence++;
        AddEvidence(amount);
    }
    public bool TrySpendMoney(int amount)
    {
        if (amount < 0 || Money < amount) return false;
        Money -= amount;
        return true;
    }
    public void ReduceHunger(float amount)
    {
        Hunger = Mathf.Max(0, Hunger - amount);

        foreach(var person in ActiveWorkers)
        {
            if(!(person.Room is Laundry)) person.Loyalty -= amount;
        }
    }
    
    public float GetTimeSpeed()
    {
        return TimeSpeedModificator * timeSpeed;
    }
    public void SetTimeSpeed(int speedModificator)
    {
        TimeSpeedModificator = speedModificator;
    }
    private IEnumerator SpawnVisitors()
    {
        for (int wave = 0; wave < visitorsCount; wave++)
        {
            var speed = GetTimeSpeed();

            if (speed > 0 && wave < visitorsCount - 1 && !reception.IsFull())
            { 
                yield return new WaitForSeconds(visitInterval / speed);
                SpawnVisitorInReception();
            }
            else {
                wave--;
                yield return null;
            }
        }
        spawnCoroutine = null;
    }
    public Person SpawnVisitor()
    {
        Person person = Instantiate(personPrefab, Canvases[1].transform);    
        person.Type = Random.value < 0.5f ? SelectedDistrict.ResidentType : (PersonType)Random.Range(0, PersonSprites.Length);
        person.Image.sprite = PersonSprites[(int)person.Type];
        return person;
    }
    private void SpawnVisitorInReception()
    {
        if (reception.IsFull()) return;
        var visitor = SpawnVisitor();
        
        reception.AssignPerson(visitor);
        Reserve.Add(visitor);
    }
    private void TriggerCthulhuEating()
    {
        var people = FindObjectsOfType<Person>();
        if (people.Length == 0) return;
        people[Random.Range(0, people.Length)]?.Eat(hungerReduction);
    }
    private void FinishShift()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        var people = FindObjectsOfType<Person>();
        foreach (var person in people)
        {
            if (!ActiveWorkers.Contains(person))
            {
                ActiveWorkers.Remove(person);
                Destroy(person.gameObject);
            }
        }
        Phase = GamePhase.Report;
        SaveManager.Save();
        Reserve.Clear();
        OpenCanvas(2);
    }
    public void NextDay()
    {
        Day++;
        TodayEvidences = 0;
        TodayInfluence = 0;        
        Phase = GamePhase.Map;
        MusicPlayer.Instance?.PlayDefaultMusic();
        OpenCanvas(0);
    }
    public void OpenCanvas(int canvasID)
    {
        for (int i = 0; i < Canvases.Length; i++) Canvases[i]?.SetActive(i == canvasID);
    }
}