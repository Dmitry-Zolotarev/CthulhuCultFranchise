using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public enum GamePhase
{
    Map,
    Office
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
    [HideInInspector] public int TodayReleased = 0;
    [HideInInspector] public int TodayEscaped = 0;
    [HideInInspector] public int TodayEaten = 0;

    [HideInInspector] public int EvidencesCount = 0;
    [HideInInspector] public float Hunger = 0;
    [HideInInspector] public District SelectedDistrict;   
    [HideInInspector] public HashSet<Person> Reserve = new HashSet<Person>();
    [HideInInspector] public HashSet<Person> ActiveWorkers = new HashSet<Person>();
    [HideInInspector] public HashSet<Person> Escaping = new HashSet<Person>();

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
    [SerializeField] private TextMeshProUGUI reduceSuspicionCostLabel;
    [SerializeField] private Image[] timeSpeedButtons;
    [SerializeField] private Color selectedButtonColor = new Color(150, 240, 100);
    [SerializeField] private TextMeshProUGUI hungerPercentLabel;
    [SerializeField] private GameObject TimeSpeedPanel;
    [SerializeField] private Slider hungerBar;
    [SerializeField] private Slider suspicionBar;
    [SerializeField] private Button reduceSuspicionButton;
    [SerializeField] private int reduceSuspicionCost = 1000;
    public GameObject StartWorkPanel;

    [Header("Prefabs")]
    public GameObject[] Canvases;
    public Sprite[] CultistSprites;
    public Sprite[] PersonSprites;
    public Transform OfficeCanvas;
    public District[] Districts;
    public Room[] Rooms;

    [SerializeField] private Person personPrefab;
    [SerializeField] private Reception reception;
    
    [Header("Balance settings")]
    public float MaxHunger = 100;
        
    [SerializeField] private int visitorsCount = 6;
    [SerializeField] private float visitInterval = 15f;
    [SerializeField] private float HungerIncreaseSpeed = 0.2f;
    [SerializeField] private float startTime = 600f;
    [SerializeField] private float endTime = 1080f;
    [SerializeField] private float timeSpeed = 6f;
    
    public float hungerReduction = 35f;
    public float MaxSuspicion = 20f;

    [Header("Audio")]
    [SerializeField] private AudioClip abyssMusic;
    [SerializeField] private AudioClip officeMusic;
    
    [HideInInspector] public float AgitationProgress = 0;
    [HideInInspector] public int TimeSpeedModificator = 1;
    
    private Coroutine spawnCoroutine;
    private void Awake()
    {
        OpenCanvas(3);
        MusicPlayer.Instance.PlayMusic(abyssMusic);
        Instance = this;
        StartWorkPanel.SetActive(false);
    }
    public void StartShift()
    {
        TodayEarned = 0;
        TodayEvidences = 0;
        TodayInfluence = 0;
        TodayReleased = 0;
        TodayEscaped = 0;
        TodayEaten = 0;

        DayTime = startTime;
        TimeSpeedModificator = 1;
        Phase = GamePhase.Office;
        StartWorkPanel.SetActive(false);

        if (TaxManager.Instance.TaxAmount > 99) AddEvidence(1);
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
        try
        {
            reduceSuspicionButton.interactable = EvidencesCount > 0 && Money >= reduceSuspicionCost;
            reduceSuspicionCostLabel?.SetText($"Снизить подозрение\n{reduceSuspicionCost}$");
        }
        catch { }
        moneyLabel?.SetText($"{Money}$");
        
        evidencesCountLabel?.SetText($"Найдено улик: {EvidencesCount}");
        todayEarnLabel?.SetText($"Доход за прошлую смену: {TodayEarned}$");
        todayTaxLabel?.SetText($"{TaxManager.Instance.TaxAmount}$");
        dayLabel?.SetText($"День {Day}");
        TimeSpeedPanel?.SetActive(Phase == GamePhase.Office);

        hungerBar.value = Hunger / MaxHunger;
        suspicionBar.value = EvidencesCount / MaxSuspicion;
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
    public void ReduceSuspicion()
    {
        if(EvidencesCount > 0 && Money >= reduceSuspicionCost)
        {
            EvidencesCount--;
            Money -= reduceSuspicionCost;
            UpdateUI();
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
    public int GetTotalInfluence()
    {
        int totalInfluence = 0;
        foreach(var district in Districts)
        {
            totalInfluence += district.Influence;
        }
        return totalInfluence;
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

    public Person InstantiatePerson()
    {
        var person = Instantiate(personPrefab, Canvases[1].transform);
        person.gameObject.SetActive(false);
        return person;
    }
    private void SpawnVisitorInReception()
    {
        if (reception.IsFull()) return;
        var visitor = InstantiatePerson();
        visitor.Type = Random.value < 0.5f ? SelectedDistrict.ResidentType : (PersonType)Random.Range(0, PersonSprites.Length);
        visitor.Image.sprite = PersonSprites[(int)visitor.Type];
        reception.AssignPerson(visitor);
        visitor.gameObject.SetActive(true);
        Reserve.Add(visitor);
    }
    private void TriggerCthulhuEating()
    {
        var people = FindObjectsOfType<Person>();
        if (people.Length == 0) return;
        people[Random.Range(0, people.Length)]?.Eat(hungerReduction);
    }
    private void Clear(HashSet<Person> collection)
    {
        foreach (var person in collection)
        {
            if (person != null) Destroy(person.gameObject);
        }
        collection.Clear();
    }
    private void FinishShift()
    {
        UpgradeWindow.Instance.gameObject.SetActive(false);
        Phase = GamePhase.Map;
        foreach (var person in FindObjectsOfType<DragPerson>())
        {
            if (!person.wasDropped)
            {
                ActiveWorkers.Remove(person.GetComponent<Person>());
                Destroy(person.gameObject);
            }
        }
        Clear(ActiveWorkers);
        Clear(Reserve);
        Clear(Escaping);
        
        OpenCanvas(2);       
    }
    public void NextDay()
    {
        Day++;      
        MusicPlayer.Instance?.PlayDefaultMusic();
        bool isMaxInfluence = true;

        foreach (var district in Districts)
        {
            if(district.Influence < 5)
            {
                isMaxInfluence = false;
                break;
            }
        }
        if(EvidencesCount >= MaxSuspicion)
        {
            OpenCanvas(4);
        }
        else if(isMaxInfluence)
        {
            OpenCanvas(5);
        }
        else OpenCanvas(0);
    }
    public void OpenCanvas(int canvasID)
    {
        if (canvasID == 0) MusicPlayer.Instance?.PlayDefaultMusic();
        for (int i = 0; i < Canvases.Length; i++) Canvases[i]?.SetActive(i == canvasID);
    }
}