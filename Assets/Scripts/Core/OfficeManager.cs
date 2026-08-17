using System.Collections;
using UnityEngine;

public class OfficeManager : MonoBehaviour
{
    public static OfficeManager Instance { get; private set; }

    [SerializeField] private Person personPrefab;
    [SerializeField] private RectTransform receptionPoint;
    [SerializeField] private Transform receptionContainer;
    [SerializeField] private int waves = 3;  
    [SerializeField] private int visitorsPerWave = 2;
    [SerializeField] private float waveInterval = 15f; 
    [SerializeField] private float startTime = 600f;
    [SerializeField] private float endTime = 1080f;
    [SerializeField] private float timeSpeed = 4f;
    private int timeSpeedModificator = 1;

    // =========================================================
    // ROOMS
    // =========================================================

    [Header("Rooms")]
    [SerializeField]
    private Room[] rooms;

    public Room[] Rooms => rooms;

    // =========================================================
    // AUDIO
    // =========================================================

    [Header("Audio")]
    [SerializeField]
    private AudioClip officeMusic;

    // =========================================================
    // INTERNAL STATE
    // =========================================================

    public bool ShiftRunning;
    private Coroutine spawnCoroutine;

    // =========================================================
    // PROPERTIES
    // =========================================================

    private void Awake()
    {
        Instance = this;
        waves++;
    }

    private void OnEnable()
    {
        MusicPlayer.Instance?.PlayMusic(officeMusic);
    }

    private void OnDisable()
    {
        MusicPlayer.Instance?.PlayDefaultMusic();
    }

    private void Update()
    {
        if (!ShiftRunning) return;
        GameManager.Instance.Time += Time.deltaTime * timeSpeed * timeSpeedModificator;
        if (GameManager.Instance.Time >= endTime) FinishShift();

        // Голод Ктулху
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddHunger(Time.deltaTime);
            if (GameManager.Instance.Hunger >= GameManager.Instance.maxHunger) TriggerCthulhuEating();
        }
    }
    
    public void StartShift()
    {
        if (ShiftRunning) return;

        GameManager.Instance.Time = 600;
        GameManager.Instance.phase = GamePhase.Office;
        GameManager.Instance.StartWorkPanel?.SetActive(false);

        ShiftRunning = true;
        // -----------------------------------------------------
        // Запускаем волны
        // -----------------------------------------------------

        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        
        spawnCoroutine = StartCoroutine(SpawnWaves());
    }
    public float GetTimeSpeed()
    {
        return timeSpeedModificator * timeSpeed;
    }
    public void SetTimeSpeed(int speedModificator)
    {
        timeSpeedModificator = speedModificator;
    }

    private IEnumerator SpawnWaves()
    {
        for (int wave = 0; wave < waves; wave++)
        {
            var speed = timeSpeed * timeSpeedModificator;

            if (speed > 0 && wave < waves - 1) 
            {      
                yield return new WaitForSeconds(waveInterval / speed);
                SpawnWave(wave + 1);
            }         
        }
        spawnCoroutine = null;
    }

    // =========================================================
    // SPAWN WAVE
    // =========================================================

    private void SpawnWave(int waveNumber)
    {
        for (int i = 0; i < visitorsPerWave; i++) SpawnVisitor();
    }

    private void SpawnVisitor()
    {
        Person person = Instantiate(personPrefab, receptionContainer);
        receptionContainer?.GetComponent<Room>()?.AssignPerson(person);

        var rect = person.GetComponent<RectTransform>();
        rect.localRotation = Quaternion.identity;
        rect.localScale = Vector3.one;
        // -----------------------------------------------------
        // Характеристики посетителя
        // -----------------------------------------------------
        person.type = Random.value < 0.5f ? PersonType.Student : PersonType.OfficeWorker;
        person.loyalty = Random.Range(1, 3);
        person.contacts = Random.Range(1, 3);
        person.Suspicion = 0;
        person.currentRoom = RoomType.Reception;

        GameManager.Instance.AddPersonToReserve(person);
    }

    // =========================================================
    // CTHULHU
    // =========================================================

    private void TriggerCthulhuEating()
    {
        var people = FindObjectsOfType<Person>();
        if (people.Length == 0) return;
        var victim = people[Random.Range(0, people.Length)];
        if (victim == null) return;
        Debug.Log($"Ктулху пожирает {victim.name}.");
        victim.Eat();
    }

    // =========================================================
    // FINISH SHIFT
    // =========================================================

    private void FinishShift()
    {
        ShiftRunning = false;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        GameManager.Instance?.NextDay();
    }
}