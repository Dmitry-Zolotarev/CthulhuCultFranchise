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
    public GamePhase phase = GamePhase.Map;

    [Header("Resources")]
    public int Money = 100;
    [HideInInspector] public int Anxiety = 0;
    [HideInInspector] public float Hunger = 0;
    [HideInInspector] public District District;   
    [HideInInspector] public HashSet<Person> reserve = new HashSet<Person>();
    [HideInInspector] public HashSet<Person> activeWorkers = new HashSet<Person>();

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
    [SerializeField] private TextMeshProUGUI hungerPercentLabel;
    [SerializeField] private Slider hungerBar;
    
    public GameObject StartWorkPanel;
    public GameObject TimeSpeedPanel;

    [Header("Prefabs")]
    public Sprite CultistSprite;
    public Sprite[] PersonSprites;  
    [SerializeField] private Person personPrefab;
    [SerializeField] private Reception reception;

    [Header("Balance settings")]
    public float MaxHunger = 100;
    [SerializeField] private float HungerIncreaseSpeed = 0.2f;
    [SerializeField] private int visitorsCount = 6;
    [SerializeField] private float visitInterval = 15f;
    [SerializeField] private float startTime = 600f;
    [SerializeField] private float endTime = 1080f;
    [SerializeField] private float timeSpeed = 6f;
    public float hungerReduction = 50f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip officeMusic;

    private int timeSpeedModificator = 1;
    private Coroutine spawnCoroutine;

    private void Awake()
    {
        Instance = this;
        StartWorkPanel.SetActive(false);
        visitorsCount++;
    }
    public void StartShift()
    {
        var people = FindObjectsOfType<DragPerson>();
        foreach (var person in people) person.ReturnToOriginalPosition();
        DayTime = startTime;
        phase = GamePhase.Office;
        StartWorkPanel.SetActive(false);
        ScreenManager.Instance.OpenMenu(1);
        MusicPlayer.Instance.PlayMusic(officeMusic);

        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnVisitors());
    }
    private void Update()
    {
        if (phase == GamePhase.Office) 
        {
            DayTime += Time.deltaTime * timeSpeed * timeSpeedModificator;
            if (DayTime >= endTime) FinishShift();

            Hunger += GetTimeSpeed() * Time.deltaTime * HungerIncreaseSpeed;
            if (Hunger > MaxHunger) TriggerCthulhuEating();        
        }  
        UpdateUI();
    }

    public void UpdateDistrictLabels()
    {
        if (District != null)
        {
            districtNameLabel?.SetText(District.Name);
            auditoryLabel?.SetText($"Аудитория: {District.Auditory.ToLower()}");
            descriptionLabel?.SetText(District.Description);
            influenceLabel?.SetText($"Влияние: {District.Influence}/5");
        }
    }
    public void UpdateUI()
    {
        foreach (var button in timeSpeedButtons) button.color = Color.white;
        switch (timeSpeedModificator)
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
        MoneyLabel?.SetText($"{Money}$");
        DayLabel?.SetText($"День {Day}");
        TimeSpeedPanel?.SetActive(phase == GamePhase.Office);

        hungerBar.value = Hunger / MaxHunger;
        hungerPercentLabel?.SetText($"{(int)(hungerBar.value * 100)}%");  

        if (phase == GamePhase.Map)
        {
            TimeLabel?.SetText("Утро");
        }
        else TimeLabel?.SetText($"{(int)DayTime / 60:D2}:{(int)DayTime % 60:D2}");
    }
    public bool TrySpendMoney(int amount)
    {
        if (amount < 0 || Money < amount) return false;
        Money -= amount;
        return true;
    }
    public void AddAnxiety(int amount)
    {
        Anxiety = Mathf.Max(0, Anxiety + amount);
    }
    public void ReduceHunger(float amount)
    {
        Hunger = Mathf.Max(0, Hunger - amount);

        foreach(var person in activeWorkers)
        {
            if(!(person.Room is Laundry)) person.loyalty -= amount;
        }
    }
    
    public float GetTimeSpeed()
    {
        return timeSpeedModificator * timeSpeed;
    }
    public void SetTimeSpeed(int speedModificator)
    {
        timeSpeedModificator = speedModificator;
    }
    private IEnumerator SpawnVisitors()
    {
        for (int wave = 0; wave < visitorsCount; wave++)
        {
            var speed = GetTimeSpeed();

            if (speed > 0 && wave < visitorsCount - 1)
            {
                yield return new WaitForSeconds((wave + 1) * visitInterval / speed);
                SpawnVisitor();
            }
            else
            {
                wave--;
                yield return null;
            }
        }
        spawnCoroutine = null;
    }
    private void SpawnVisitor()
    {
        Person person = Instantiate(personPrefab, reception.transform);
        reception.AssignPerson(person);

        person.Type = Random.value < 0.5f ? District.ResidentType : (PersonType)Random.Range(0, PersonSprites.Length);
        person.personImage.sprite = PersonSprites[(int)person.Type];
        reserve.Add(person);
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
        NextDay();
    }
    public void NextDay()
    {
        Day++;
        foreach (var person in reserve) 
        {
            if(!activeWorkers.Contains(person)) Destroy(person.gameObject);         
        }    
        reserve.Clear();
        phase = GamePhase.Map;
        MusicPlayer.Instance.PlayDefaultMusic();
        ScreenManager.Instance.OpenMenu(0);
    }
}