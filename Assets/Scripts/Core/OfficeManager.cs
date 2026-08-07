using System.Collections;
using UnityEngine;

public class OfficeManager : MonoBehaviour
{
    public static OfficeManager Instance { get; private set; }

    public Person personPrefab;
    public Transform receptionPoint;

    public float shiftDuration = 60f;
    public int waves = 3;
    public int visitorsPerWave = 2;
    public float waveInterval = 15f;

    private float shiftTimer;
    private bool shiftRunning;
    private int extraVisitors;
    [SerializeField] private AudioClip officeMusic;

    private void Awake()
    {
        Instance = this;
        
    }
    private void OnEnable()
    {
        MusicPlayer.Instance.PlayMusic(officeMusic);
    }
    private void OnDisable()
    {
        MusicPlayer.Instance.PlayDefaultMusic();
    }
    public void StartShift()
    {
        if (shiftRunning) return;

        GameManager.Instance.phase = GamePhase.Office;
        shiftTimer = shiftDuration;
        shiftRunning = true;

        StartCoroutine(SpawnWaves());
    }

    private void Update()
    {
        if (!shiftRunning) return;

        shiftTimer -= Time.deltaTime;

        GameManager.Instance.AddHunger(
            Mathf.RoundToInt(Time.deltaTime)
        );

        if (GameManager.Instance.hunger >= GameManager.Instance.maxHunger)
            TriggerCthulhuEating();

        if (shiftTimer <= 0)
            FinishShift();
    }

    private IEnumerator SpawnWaves()
    {
        for (int i = 0; i < waves; i++)
        {
            SpawnWave();
            yield return new WaitForSeconds(waveInterval);
        }
    }

    private void SpawnWave()
    {
        int count = visitorsPerWave + extraVisitors;

        for (int i = 0; i < count; i++)
            SpawnVisitor();

        extraVisitors = 0;
    }

    private void SpawnVisitor()
    {
        if (personPrefab == null)
        {
            Debug.LogError("Person Prefab не назначен.");
            return;
        }

        Person person = Instantiate(
            personPrefab,
            receptionPoint.position,
            Quaternion.identity
        );

        person.type = Random.value < 0.5f
            ? PersonType.Student
            : PersonType.OfficeWorker;

        person.loyalty = Random.Range(1, 3);
        person.contacts = Random.Range(1, 3);
    }

    public void AddExtraVisitors(int amount) =>
        extraVisitors += amount;

    public void SignVisitor(Person person)
    {
        if (person == null) return;

        person.SignContract();
        GameManager.Instance.AddPersonToReserve(person);

        Debug.Log($"{person.name} подписал договор и стал культистом.");
    }

    private void FinishShift()
    {
        if (!shiftRunning) return;

        shiftRunning = false;
        StopAllCoroutines();

        GameManager.Instance.EndDay();
        Debug.Log("Смена закончена. Переход к отчёту.");
    }

    private void TriggerCthulhuEating()
    {
        Person[] people = FindObjectsOfType<Person>();

        if (people.Length == 0) return;

        Person victim = people[Random.Range(0, people.Length)];

        Debug.Log($"Ктулху пожирает {victim.name}.");
        victim.Eat();
    }

    public float GetRemainingTime() =>
        Mathf.Max(0, shiftTimer);
}
