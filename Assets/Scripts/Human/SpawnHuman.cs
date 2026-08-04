using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHuman : MonoBehaviour
{
    [Header("Spawn points")]
    [SerializeField] private Transform spawnpoint;

    [Header("Prefabs: 0-worker 1-student 2-blogger 3-esoteric 4-retiree 5-police")]
    [SerializeField] private GameObject[] human;

    [Header("Balance asset")]
    [SerializeField] private ListReactions listReactions;

    [Header("Optional links")]
    [SerializeField] private DayController dayController;
    [SerializeField] private GameSessionBridge bridge;

    [Header("Wave spawn settings")]
    [SerializeField] private int maxHumansOnScreen = 5;
    [SerializeField] private float phaseStartShare = 0.25f;
    [SerializeField] private float phaseMainShare = 0.55f;
    [SerializeField] private float groupStartOffset = 0.7f;
    [SerializeField] private float groupEndReserve = 0.3f;
    [SerializeField] private float spawnTimeJitter = 0.35f;
    [SerializeField] private float delayInsideGroupMin = 0.15f;
    [SerializeField] private float delayInsideGroupMax = 0.45f;

    [Header("Police")]
    [SerializeField] private float policeSpawnInterval = 7f;

    private readonly Dictionary<string, int> nameToIndex = new Dictionary<string, int>()
    {
        {"worker", 0},
        {"student", 1},
        {"blogger", 2},
        {"esoteric", 3},
        {"retiree", 4}
    };

    private readonly Dictionary<string, float> weights = new Dictionary<string, float>()
    {
        {"worker", 24f},
        {"student", 24f},
        {"retiree", 18f},
        {"blogger", 17f},
        {"esoteric", 17f}
    };

    
    private readonly List<SpawnGroup> spawnSchedule = new List<SpawnGroup>();
    public readonly List<GameObject> spawnedPeople = new List<GameObject>();

    private bool canSpawn;
    private int currentDayIndex = 0;
    private float currentDayDuration = 24f;
    private Coroutine daySpawnRoutine;
    private Coroutine policeRoutine;

    private struct SpawnGroup
    {
        public float time;
        public int size;

        public SpawnGroup(float time, int size)
        {
            this.time = time;
            this.size = size;
        }
    }

    public int ActiveSpawnedCount
    {
        get
        {
            CleanupNulls();
            return spawnedPeople.Count;
        }
    }

    public bool IsSpawning => canSpawn;

    private void Awake()
    {
        if (bridge == null)
            bridge = GameSessionBridge.Instance;
    }

    private void Start()
    {
        // Не стартуем сами. Старт только от кнопки через DayController.dayEnd(false).
        canSpawn = false;
    }

    private void OnEnable()
    {
        DayController.dayEnd += OnDayStateChanged;
    }

    private void OnDisable()
    {
        DayController.dayEnd -= OnDayStateChanged;
        StopSpawnRoutines();
    }

    private void OnDayStateChanged(bool ended)
    {
        if (ended)
        {
            StopNewSpawnsOnly();
            return;
        }

        StartDaySpawn();
    }

    public void StartDaySpawn()
    {
        StopSpawnRoutines();

        if (bridge == null)
            bridge = GameSessionBridge.Instance;

        currentDayIndex = GetCurrentDayIndex();

        if (!HasDayData(currentDayIndex))
        {
            canSpawn = false;
            return;
        }

        canSpawn = true;
        currentDayDuration = Mathf.Max(1f, GetDayDuration(currentDayIndex));
        int totalHumansForDay = GetHumanCountForDay(currentDayIndex);

        ApplyBridgeWeightModifiers();
        BuildWaveSchedule(totalHumansForDay, currentDayDuration, currentDayIndex);

        daySpawnRoutine = StartCoroutine(SpawnDayRoutine());

        if (currentDayIndex > 0 && HasPolicePrefab())
            policeRoutine = StartCoroutine(PoliceRoutine());
    }

    public void StopNewSpawnsOnly()
    {
        canSpawn = false;
        StopSpawnRoutines();
    }

    public void ClearAllSpawned()
    {
        for (int i = spawnedPeople.Count - 1; i >= 0; i--)
        {
            if (spawnedPeople[i] != null)
                Destroy(spawnedPeople[i]);
        }

        spawnedPeople.Clear();
    }

    private int GetCurrentDayIndex()
    {
        if (dayController != null)
            return Mathf.Max(0, dayController.CurrentDayNumber - 1);

        if (bridge != null)
            return Mathf.Max(0, bridge.CurrentDay - 1);

        return currentDayIndex;
    }

    private int GetDayDuration(int dayIndex)
    {
        if (bridge != null)
            return bridge.BuildStreetPlan().duration;

        if (listReactions == null || listReactions.LenDaySec == null || listReactions.LenDaySec.Length == 0)
            return 30;

        dayIndex = Mathf.Clamp(dayIndex, 0, listReactions.LenDaySec.Length - 1);
        return listReactions.LenDaySec[dayIndex];
    }

    private int GetHumanCountForDay(int dayIndex)
    {
        if (bridge != null)
            return bridge.BuildStreetPlan().finalHumans;

        if (listReactions == null || listReactions.CountHuman == null || listReactions.CountHuman.Length == 0)
            return 10;

        dayIndex = Mathf.Clamp(dayIndex, 0, listReactions.CountHuman.Length - 1);
        return listReactions.CountHuman[dayIndex];
    }

    private void ApplyBridgeWeightModifiers()
    {
        weights["worker"] = 24f;
        weights["student"] = 24f;
        weights["retiree"] = 18f;
        weights["blogger"] = 17f;
        weights["esoteric"] = 17f;

        if (bridge != null && bridge.HasCthulhuMerch)
            weights["blogger"] = 25f;
    }

    private void StopSpawnRoutines()
    {
        if (daySpawnRoutine != null)
        {
            StopCoroutine(daySpawnRoutine);
            daySpawnRoutine = null;
        }

        if (policeRoutine != null)
        {
            StopCoroutine(policeRoutine);
            policeRoutine = null;
        }
    }

    private bool HasDayData(int dayIndex)
    {
        if (listReactions == null)
            return bridge != null;

        if (listReactions.LenDaySec == null || listReactions.CountHuman == null)
            return bridge != null;

        return dayIndex >= 0 && dayIndex < listReactions.LenDaySec.Length && dayIndex < listReactions.CountHuman.Length;
    }

    private bool HasPolicePrefab()
    {
        return human != null && human.Length > 5 && human[5] != null;
    }

    private void BuildWaveSchedule(int totalHumans, float dayDuration, int dayIndex)
    {
        spawnSchedule.Clear();

        int startCount = Mathf.RoundToInt(totalHumans * 0.20f);
        int finalCount = Mathf.RoundToInt(totalHumans * 0.25f);
        int mainCount = totalHumans - startCount - finalCount;

        if (mainCount < 0)
        {
            mainCount = 0;
            finalCount = Mathf.Max(0, totalHumans - startCount);
        }

        float startDuration = dayDuration * phaseStartShare;
        float mainDuration = dayDuration * phaseMainShare;
        float finalDuration = Mathf.Max(0f, dayDuration - startDuration - mainDuration);

        AddPhaseGroups(0f, startDuration, startCount, dayIndex);
        AddPhaseGroups(startDuration, mainDuration, mainCount, dayIndex);
        AddPhaseGroups(startDuration + mainDuration, finalDuration, finalCount, dayIndex);

        spawnSchedule.Sort((a, b) => a.time.CompareTo(b.time));
    }

    private void AddPhaseGroups(float phaseStart, float phaseDuration, int humanCount, int dayIndex)
    {
        if (humanCount <= 0 || phaseDuration <= 0f)
            return;

        List<int> groups = CreateGroupSizes(humanCount, dayIndex);
        int groupCount = groups.Count;

        if (groupCount <= 0)
            return;

        float interval = phaseDuration / groupCount;

        for (int i = 0; i < groupCount; i++)
        {
            float baseTime = phaseStart + groupStartOffset + i * interval;
            float randomOffset = Random.Range(-spawnTimeJitter, spawnTimeJitter);
            float time = baseTime + randomOffset;

            float minTime = phaseStart;
            float maxTime = phaseStart + phaseDuration - groupEndReserve;

            if (maxTime < minTime)
                maxTime = minTime;

            time = Mathf.Clamp(time, minTime, maxTime);
            spawnSchedule.Add(new SpawnGroup(time, groups[i]));
        }
    }

    private List<int> CreateGroupSizes(int humanCount, int dayIndex)
    {
        List<int> groups = new List<int>();
        int remaining = humanCount;

        while (remaining > 0)
        {
            int size = ChooseGroupSize(dayIndex, remaining);
            groups.Add(size);
            remaining -= size;
        }

        return groups;
    }

    private int ChooseGroupSize(int dayIndex, int remaining)
    {
        int maxGroupSize = dayIndex < 2 ? 2 : 3;
        maxGroupSize = Mathf.Min(maxGroupSize, remaining);

        float singleWeight;
        float pairWeight;
        float tripleWeight;

        switch (dayIndex)
        {
            case 0:
                singleWeight = 75f; pairWeight = 25f; tripleWeight = 0f;
                break;
            case 1:
                singleWeight = 60f; pairWeight = 40f; tripleWeight = 0f;
                break;
            case 2:
                singleWeight = 40f; pairWeight = 45f; tripleWeight = 15f;
                break;
            case 3:
                singleWeight = 30f; pairWeight = 50f; tripleWeight = 20f;
                break;
            default:
                singleWeight = 25f; pairWeight = 50f; tripleWeight = 25f;
                break;
        }

        if (maxGroupSize < 3)
            tripleWeight = 0f;
        if (maxGroupSize < 2)
            pairWeight = 0f;

        float total = singleWeight + pairWeight + tripleWeight;
        float roll = Random.Range(0f, total);

        if (roll <= singleWeight)
            return 1;

        if (roll <= singleWeight + pairWeight)
            return Mathf.Min(2, maxGroupSize);

        return Mathf.Min(3, maxGroupSize);
    }

    private IEnumerator SpawnDayRoutine()
    {
        float elapsed = 0f;
        int scheduleIndex = 0;

        while (canSpawn && scheduleIndex < spawnSchedule.Count)
        {
            elapsed += Time.deltaTime;

            while (canSpawn && scheduleIndex < spawnSchedule.Count && elapsed >= spawnSchedule[scheduleIndex].time)
            {
                yield return StartCoroutine(SpawnGroupRoutine(spawnSchedule[scheduleIndex].size));
                scheduleIndex++;
            }

            yield return null;
        }

        daySpawnRoutine = null;
    }

    private IEnumerator SpawnGroupRoutine(int groupSize)
    {
        for (int i = 0; i < groupSize; i++)
        {
            if (!canSpawn)
                yield break;

            while (canSpawn && ActiveSpawnedCount >= maxHumansOnScreen)
                yield return null;

            if (!canSpawn)
                yield break;

            SpawnOneHuman();

            if (i < groupSize - 1)
                yield return new WaitForSeconds(Random.Range(delayInsideGroupMin, delayInsideGroupMax));
        }
    }

    private IEnumerator PoliceRoutine()
    {
        yield return new WaitForSeconds(policeSpawnInterval * 0.5f);

        while (canSpawn)
        {
            SpawnPolice();
            yield return new WaitForSeconds(policeSpawnInterval);
        }
    }

    private void SpawnOneHuman()
    {
        GameObject prefab = GetRandomHumanByWeight();
        SpawnPrefab(prefab);
    }

    private void SpawnPolice()
    {
        if (!HasPolicePrefab())
            return;

        SpawnPrefab(human[5]);
    }

    private GameObject SpawnPrefab(GameObject prefab)
    {
        if (prefab == null || spawnpoint == null)
            return null;

        GameObject obj = Instantiate(prefab, spawnpoint.position, spawnpoint.rotation);
        spawnedPeople.Add(obj);
        return obj;
    }

    public void SetWeight(string type, float newWeight)
    {
        if (weights.ContainsKey(type))
            weights[type] = Mathf.Max(0f, newWeight);
    }

    private GameObject GetRandomHumanByWeight()
    {
        if (human == null || human.Length == 0)
            return null;

        float totalWeight = 0f;
        foreach (float w in weights.Values)
            totalWeight += w;

        float randomPoint = Random.Range(0f, totalWeight);
        float current = 0f;

        foreach (KeyValuePair<string, float> pair in weights)
        {
            current += pair.Value;
            if (randomPoint <= current)
            {
                int index = nameToIndex[pair.Key];
                if (index >= 0 && index < human.Length && human[index] != null)
                    return human[index];
            }
        }

        return human[0];
    }

    public void DeleteHuman(GameObject hum)
    {
        if (hum == null)
            return;

        spawnedPeople.Remove(hum);
        Destroy(hum);
    }

    private void CleanupNulls()
    {
        for (int i = spawnedPeople.Count - 1; i >= 0; i--)
        {
            if (spawnedPeople[i] == null)
                spawnedPeople.RemoveAt(i);
        }
    }
}
