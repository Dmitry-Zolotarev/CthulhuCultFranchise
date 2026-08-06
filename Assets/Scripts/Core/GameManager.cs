using System.Collections.Generic;
using UnityEngine;

public enum GamePhase { Map, Preparation, Office, Report, Final }
public enum PersonType { Student, OfficeWorker }
public enum RoomType { Reception, Donations, Propaganda, Laundry, Altar }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int day = 1;
    public GamePhase phase = GamePhase.Map;

    public int money = 600;
    public int influence = 2;
    public int grace = 0;
    public int anxiety = 0;
    public int hunger = 0;

    public int maxInfluence = 4;
    public int maxHunger = 100;

    public List<Person> reserve = new List<Person>();
    public List<Person> activeWorkers = new List<Person>();

    public int universityProgress;
    public int businessProgress;

    public bool kpiStarted;
    public int kpiMoney;
    public int kpiContacts;

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

    public void StartNewGame()
    {
        day = 1;
        money = 600;
        influence = 2;
        grace = 0;
        anxiety = 0;
        hunger = 0;

        reserve.Clear();
        activeWorkers.Clear();

        universityProgress = 0;
        businessProgress = 0;

        kpiStarted = false;
        kpiMoney = 0;
        kpiContacts = 0;

        phase = GamePhase.Map;
    }

    public bool SpendMoney(int amount)
    {
        if (money < amount) return false;
        money -= amount;
        return true;
    }

    public bool SpendInfluence(int amount)
    {
        if (influence < amount) return false;
        influence -= amount;
        return true;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        if (kpiStarted && amount > 0) kpiMoney += amount;
    }

    public void AddGrace(int amount) => grace += amount;

    public void AddAnxiety(int amount) =>
        anxiety = Mathf.Max(0, anxiety + amount);

    public void AddHunger(int amount) =>
        hunger = Mathf.Clamp(hunger + amount, 0, maxHunger);

    public void ReduceHunger(int amount) =>
        hunger = Mathf.Max(0, hunger - amount);

    public void AddInfluence(int amount) =>
        influence = Mathf.Clamp(influence + amount, 0, maxInfluence);

    public void StartKPI()
    {
        kpiStarted = true;
        kpiMoney = 0;
        kpiContacts = 0;
    }

    public void RegisterContactConverted()
    {
        if (kpiStarted) kpiContacts++;
    }

    public bool IsKPIComplete() =>
        kpiMoney >= 400 && kpiContacts >= 2 && hunger < 75;

    public void AddPersonToReserve(Person person)
    {
        if (reserve.Count >= 8)
        {
            Destroy(person.gameObject);
            return;
        }

        person.currentRoom = RoomType.Reception;

        if (!reserve.Contains(person))
            reserve.Add(person);
    }

    public void RemoveFromReserve(Person person) =>
        reserve.Remove(person);

    public void EndDay() => phase = GamePhase.Report;

    public void NextDay()
    {
        if (day >= 3)
        {
            phase = GamePhase.Final;
            return;
        }

        day++;
        AddInfluence(2);
        activeWorkers.Clear();

        foreach (Person person in reserve)
            person.Suspicion = Mathf.Max(0, person.Suspicion - 1);

        phase = GamePhase.Map;
    }
}
