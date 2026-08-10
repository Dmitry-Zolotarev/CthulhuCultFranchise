using System.Collections;
using UnityEngine;

public class OfficeManager : MonoBehaviour
{
    public static OfficeManager Instance { get; private set; }

    // =========================================================
    // PERSON
    // =========================================================

    [Header("Person")]
    [SerializeField]
    private Person personPrefab;

    // =========================================================
    // RECEPTION
    // =========================================================

    [Header("Reception")]
    [SerializeField]
    private RectTransform receptionPoint;

    [SerializeField]
    private Transform receptionContainer;

    // =========================================================
    // SHIFT
    // =========================================================

    [Header("Shift")]
    [SerializeField]
    private float shiftDuration = 60f;

    [SerializeField]
    private int waves = 3;

    [SerializeField]
    private int visitorsPerWave = 2;

    [SerializeField]
    private float waveInterval = 15f;

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

    private float shiftTimer;

    private bool shiftRunning;

    private int extraVisitors;

    private Coroutine spawnCoroutine;

    // =========================================================
    // PROPERTIES
    // =========================================================

    public bool IsShiftRunning =>
        shiftRunning;

    public float RemainingTime =>
        Mathf.Max(
            0f,
            shiftTimer
        );

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        FindRoomsIfNeeded();
    }

    private void OnEnable()
    {
        if (MusicPlayer.Instance != null &&
            officeMusic != null)
        {
            MusicPlayer.Instance.PlayMusic(
                officeMusic
            );
        }
    }

    private void OnDisable()
    {
        if (MusicPlayer.Instance != null)
        {
            MusicPlayer.Instance.PlayDefaultMusic();
        }
    }

    private void Update()
    {
        if (!shiftRunning)
            return;

        // -----------------------------------------------------
        // Таймер
        // -----------------------------------------------------

        shiftTimer -=
            Time.deltaTime;

        // -----------------------------------------------------
        // Голод
        // -----------------------------------------------------

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddHunger(
                Mathf.RoundToInt(
                    Time.deltaTime
                )
            );

            // -------------------------------------------------
            // Ктулху
            // -------------------------------------------------

            if (GameManager.Instance.Hunger >=
                GameManager.Instance.maxHunger)
            {
                TriggerCthulhuEating();
            }
        }

        // -----------------------------------------------------
        // Конец смены
        // -----------------------------------------------------

        if (shiftTimer <= 0f)
        {
            FinishShift();
        }
    }

    // =========================================================
    // ROOMS
    // =========================================================

    private void FindRoomsIfNeeded()
    {
        if (rooms != null &&
            rooms.Length > 0)
        {
            return;
        }

        rooms =
            FindObjectsOfType<Room>(
                true
            );
    }

    // =========================================================
    // START SHIFT
    // =========================================================

    public void StartShift()
    {
        if (shiftRunning)
            return;

        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "OfficeManager: GameManager не найден."
            );

            return;
        }

        if (personPrefab == null)
        {
            Debug.LogError(
                "OfficeManager: Person Prefab не назначен."
            );

            return;
        }

        if (receptionPoint == null)
        {
            Debug.LogError(
                "OfficeManager: Reception Point не назначен."
            );

            return;
        }

        // -----------------------------------------------------
        // Переводим игру в офисную фазу
        // -----------------------------------------------------

        GameManager.Instance.phase =
            GamePhase.Office;

        // -----------------------------------------------------
        // Запускаем смену
        // -----------------------------------------------------

        shiftTimer =
            shiftDuration;

        shiftRunning = true;

        // На всякий случай сбрасываем
        // бонус предыдущей смены.
        extraVisitors = 0;

        Debug.Log(
            $"Офисная смена началась. " +
            $"День: {GameManager.Instance.day}"
        );

        // -----------------------------------------------------
        // Запускаем волны
        // -----------------------------------------------------

        if (spawnCoroutine != null)
        {
            StopCoroutine(
                spawnCoroutine
            );
        }

        spawnCoroutine =
            StartCoroutine(
                SpawnWaves()
            );
    }

    // =========================================================
    // SPAWN WAVES
    // =========================================================

    private IEnumerator SpawnWaves()
    {
        for (int wave = 0;
             wave < waves;
             wave++)
        {
            if (!shiftRunning)
                yield break;

            SpawnWave(
                wave + 1
            );

            if (wave < waves - 1)
            {
                yield return new WaitForSeconds(
                    waveInterval
                );
            }
        }

        spawnCoroutine = null;
    }

    // =========================================================
    // SPAWN WAVE
    // =========================================================

    private void SpawnWave(
        int waveNumber)
    {
        int count =
            visitorsPerWave +
            extraVisitors;

        // Бонус применяется только
        // к этой следующей волне.
        extraVisitors = 0;

        Debug.Log(
            $"Волна {waveNumber}: " +
            $"приходит {count} посетителей."
        );

        for (int i = 0;
             i < count;
             i++)
        {
            SpawnVisitor();
        }
    }

    // =========================================================
    // SPAWN VISITOR
    // =========================================================

    private void SpawnVisitor()
    {
        if (personPrefab == null)
        {
            Debug.LogError(
                "OfficeManager: Person Prefab не назначен."
            );

            return;
        }

        if (receptionPoint == null)
        {
            Debug.LogError(
                "OfficeManager: Reception Point не назначен."
            );

            return;
        }

        Person person;

        // -----------------------------------------------------
        // Создаём человека в Reception
        // -----------------------------------------------------

        if (receptionContainer != null)
        {
            person = Instantiate(
                personPrefab,
                receptionContainer
            );
        }
        else
        {
            person = Instantiate(
                personPrefab,
                receptionPoint
            );
        }

        if (person == null)
            return;

        // -----------------------------------------------------
        // UI transform
        // -----------------------------------------------------

        RectTransform rect =
            person.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchoredPosition =
                GetSpawnPosition();

            rect.localRotation =
                Quaternion.identity;

            rect.localScale =
                Vector3.one;
        }

        // -----------------------------------------------------
        // Характеристики посетителя
        // -----------------------------------------------------

        person.type =
            Random.value < 0.5f
                ? PersonType.Student
                : PersonType.OfficeWorker;

        person.loyalty =
            Random.Range(1, 3);

        person.contacts =
            Random.Range(1, 3);

        person.Suspicion = 0;

        person.currentRoom =
            RoomType.Reception;

        // -----------------------------------------------------
        // Добавляем в резерв
        // -----------------------------------------------------

        GameManager.Instance.AddPersonToReserve(
            person
        );

        Debug.Log(
            $"Посетитель {person.name} " +
            $"пришёл в приёмную."
        );
    }

    // =========================================================
    // SPAWN POSITION
    // =========================================================

    private Vector2 GetSpawnPosition()
    {
        if (receptionPoint == null)
            return Vector2.zero;

        Rect rect =
            receptionPoint.rect;

        float x =
            Random.Range(
                rect.xMin,
                rect.xMax
            );

        float y =
            Random.Range(
                rect.yMin,
                rect.yMax
            );

        return new Vector2(
            x,
            y
        );
    }

    // =========================================================
    // EXTRA VISITORS
    // =========================================================

    public void AddExtraVisitors(
        int amount)
    {
        if (amount <= 0)
            return;

        extraVisitors += amount;

        Debug.Log(
            $"К следующей волне добавлено " +
            $"{amount} посетителей."
        );
    }

    // =========================================================
    // SIGN VISITOR
    // =========================================================

    public void SignVisitor(
        Person person)
    {
        if (person == null)
            return;

        if (GameManager.Instance == null)
            return;

        person.SignContract();

        GameManager.Instance.AddPersonToReserve(
            person
        );

        GameManager.Instance.RegisterContactConverted();

        Debug.Log(
            $"{person.name} подписал договор " +
            $"и стал культистом."
        );
    }

    // =========================================================
    // CTHULHU
    // =========================================================

    private void TriggerCthulhuEating()
    {
        Person[] people =
            FindObjectsOfType<Person>();

        if (people.Length == 0)
            return;

        Person victim =
            people[
                Random.Range(
                    0,
                    people.Length
                )
            ];

        if (victim == null)
            return;

        Debug.Log(
            $"Ктулху пожирает {victim.name}."
        );

        victim.Eat();
    }

    // =========================================================
    // FINISH SHIFT
    // =========================================================

    private void FinishShift()
    {
        if (!shiftRunning)
            return;

        shiftRunning = false;

        if (spawnCoroutine != null)
        {
            StopCoroutine(
                spawnCoroutine
            );

            spawnCoroutine = null;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndDay();
        }

        Debug.Log(
            "Смена закончена. " +
            "Переход к отчёту."
        );
    }

    // =========================================================
    // TIME
    // =========================================================

    public float GetRemainingTime()
    {
        return Mathf.Max(
            0f,
            shiftTimer
        );
    }
}