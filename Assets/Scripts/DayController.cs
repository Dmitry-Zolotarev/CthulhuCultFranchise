using System;
using UnityEngine;
using TMPro;

public class DayController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private ListReactions listReactions;
    [SerializeField, Min(1)] private int startDay = 1;

    [Header("TMP UI")]
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text timerText;

    // false = улица стартовала, true = день реально завершён и можно показывать итог.
    public static event Action<bool> dayEnd;

    // Срабатывает, когда таймер дошёл до 0. Важно: итоговую панель сразу НЕ открываем.
    // StreetDayFlowController после этого останавливает новый спавн и ждёт, пока текущие прохожие уйдут.
    public static event Action timerExpired;

    private int currentDayIndex;
    private float timer;
    private bool isDayRunning;
    private bool waitingForNextDay;
    private bool timerExpiredAlready;

    public int CurrentDayNumber => currentDayIndex + 1;
    public int CurrentDayIndex => currentDayIndex;
    public float CurrentTimer => timer;
    public bool IsDay => isDayRunning;
    public bool WaitingForNextDay => waitingForNextDay;
    public string LastEndReason { get; private set; } = "День завершён";

    private void Start()
    {
        currentDayIndex = Mathf.Clamp(startDay - 1, 0, GetLastDayIndex());
        timer = GetDayDuration(currentDayIndex);
        isDayRunning = false;
        waitingForNextDay = false;
        timerExpiredAlready = false;
        UpdateUI();
    }

    private void Update()
    {
        if (!isDayRunning)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = 0f;
            UpdateTimerText();
            ExpireTimer("Время вышло");
            return;
        }

        UpdateTimerText();
    }

    public void StartDay()
    {
        if (waitingForNextDay)
        {

            currentDayIndex++;
        }

        StartCurrentDay();
    }

    public void StartDay(int dayNumber)
    {
        currentDayIndex = Mathf.Clamp(dayNumber - 1, 0, GetLastDayIndex());
        StartCurrentDay();
    }

    public void SetDay(int dayNumber)
    {
        currentDayIndex = Mathf.Clamp(dayNumber - 1, 0, GetLastDayIndex());
        timer = GetDayDuration(currentDayIndex);
        isDayRunning = false;
        waitingForNextDay = false;
        timerExpiredAlready = false;
        LastEndReason = "День подготовлен";
        UpdateUI();
    }

    public void RestartCurrentDay()
    {
        StartCurrentDay();
    }

    public void StopTimerOnly()
    {
        isDayRunning = false;
        timer = 0f;
        UpdateTimerText();
    }

    public void ExpireTimer(string reason)
    {
        if (timerExpiredAlready)
            return;

        isDayRunning = false;
        timerExpiredAlready = true;
        LastEndReason = string.IsNullOrEmpty(reason) ? "Время вышло" : reason;
        Debug.Log("Таймер улицы закончился. Ждём, пока прохожие уйдут.");
        timerExpired?.Invoke();
    }

    // Использовать для полиции/ручного завершения, когда день должен закончиться сразу.
    public void ForceEndDay(string reason)
    {
        CompleteDay(reason);
    }

    // Вызывать, когда реально можно открыть итоговую панель.
    public void CompleteDay(string reason)
    {
        isDayRunning = false;
        waitingForNextDay = true;
        timerExpiredAlready = false;
        timer = 0f;
        LastEndReason = string.IsNullOrEmpty(reason) ? "День завершён" : reason;
        UpdateUI();
        Debug.Log("День завершён: " + CurrentDayNumber + " | Причина: " + LastEndReason);
        dayEnd?.Invoke(true);
    }

    private void StartCurrentDay()
    {
        timer = GetDayDuration(currentDayIndex);
        isDayRunning = true;
        waitingForNextDay = false;
        timerExpiredAlready = false;
        LastEndReason = "День идёт";
        UpdateUI();

        Debug.Log("Старт улицы. День: " + CurrentDayNumber);
        dayEnd?.Invoke(false);
    }

    private int GetLastDayIndex()
    {
        if (listReactions == null || listReactions.LenDaySec == null || listReactions.LenDaySec.Length == 0)
            return 0;

        return listReactions.LenDaySec.Length - 1;
    }

    public int GetDayDurationByNumber(int dayNumber)
    {
        return GetDayDuration(Mathf.Clamp(dayNumber - 1, 0, GetLastDayIndex()));
    }

    private int GetDayDuration(int index)
    {
        if (listReactions == null || listReactions.LenDaySec == null || listReactions.LenDaySec.Length == 0) return 60;


        index = Mathf.Clamp(index, 0, listReactions.LenDaySec.Length - 1);
        return listReactions.LenDaySec[index];
    }

    private void UpdateUI()
    {
        if (dayText != null) dayText.text = "День " + CurrentDayNumber;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timer);
            timerText.text = seconds.ToString();
        }       
    }
}
