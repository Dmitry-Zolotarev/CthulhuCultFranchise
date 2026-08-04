using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CounterHuman : MonoBehaviour
{
    [Header("TMP UI")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text messaVisitorsText;

    [Header("Street money")]
    [SerializeField] private bool addMoneyOnStreet = false;
    [SerializeField] private int startMoney = 0;
    [SerializeField] private int workerIncome = 4;
    [SerializeField] private int studentIncome = 1;
    [SerializeField] private int retireeIncome = 2;
    [SerializeField] private int bloggerIncome = 2;
    [SerializeField] private int esotericIncome = 2;

    private int todayVisitors = 0;
    private int currentMoney = 0;
    private readonly Dictionary<string, int> countRecruit = new Dictionary<string, int>();

    public int CurrentMoney => GameSessionBridge.Instance != null ? GameSessionBridge.Instance.CurrentMoney : currentMoney;
    public int GetTodayVisitors() => GameSessionBridge.Instance != null ? GameSessionBridge.Instance.TotalVisitors : todayVisitors;
    public int GetTotalRecruited() => GetTodayVisitors();

    private void Start()
    {
        currentMoney = startMoney;
        UpdateUI();
    }

    public void AddHuman(int add, string humanType)
    {
        if (add <= 0)
            return;

        string key = NormalizeType(humanType);
        todayVisitors += add;

        if (countRecruit.ContainsKey(key))
            countRecruit[key] += add;
        else
            countRecruit.Add(key, add);

        if (addMoneyOnStreet)
        {
            int income = GetIncome(key) * add;
            currentMoney += income;
            if (GameSessionBridge.Instance != null)
                GameSessionBridge.Instance.Money += income;
        }

        Debug.Log("В мессе: " + todayVisitors + " | Тип: " + key + " | Кол-во типа: " + countRecruit[key]);
        UpdateUI();
    }

    public void ResetToday()
    {
        todayVisitors = 0;
        countRecruit.Clear();
        UpdateUI();
    }

    private int GetIncome(string key)
    {
        switch (key)
        {
            case "worker": return workerIncome;
            case "student": return studentIncome;
            case "retiree": return retireeIncome;
            case "blogger": return bloggerIncome;
            case "esoteric": return esotericIncome;
            default: return 0;
        }
    }

    private string NormalizeType(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "unknown";

        value = value.ToLower().Replace("(clone)", "").Trim();
        if (value.Contains("worker")) return "worker";
        if (value.Contains("student")) return "student";
        if (value.Contains("retiree") || value.Contains("pension")) return "retiree";
        if (value.Contains("blogger")) return "blogger";
        if (value.Contains("esoteric")) return "esoteric";
        return value;
    }

    private void UpdateUI()
    {
        if (messaVisitorsText != null) messaVisitorsText.text = $"{GetTodayVisitors()}";
    }
}
