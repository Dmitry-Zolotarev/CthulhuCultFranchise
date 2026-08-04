using System;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    public static ComboSystem Instance { get; private set; }

    [Serializable]
    public class ComboReward
    {
        public int combo = 3;
        public int money = 1;
    }

    [Header("UI")]
    [SerializeField] private TMP_Text comboText;
    [Tooltip("Можно оставить тот же MoneyText, что у GameSessionBridge. ComboSystem больше НЕ будет перезаписывать деньги в 0, если выключен Control Money Text Directly.")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text moneyPopupText;

    [Header("Combo")]
    [SerializeField] private string comboPrefix = "Комбо: ";
    [SerializeField] private ComboReward[] rewards = new ComboReward[]
    {
        new ComboReward { combo = 3, money = 1 },
        new ComboReward { combo = 7, money = 5 },
        new ComboReward { combo = 11, money = 10 }
    };

    [Header("Money logic")]
    [Tooltip("ВЫКЛ для основной игры. Если выключено, деньги считает GameSessionBridge, а ComboSystem только даёт бонус и анимирует текст.")]
    [SerializeField] private bool controlMoneyTextDirectly = false;

    [Tooltip("Используется только если Control Money Text Directly включён или если нет GameSessionBridge/moneyReceiver.")]
    [SerializeField] private int startMoney = 0;

    [Header("Optional money receiver")]
    [Tooltip("Перетащи сюда GameSessionBridge. Если пусто, ComboSystem сам попробует найти GameSessionBridge.Instance.")]
    [SerializeField] private MonoBehaviour moneyReceiver;
    [SerializeField] private string addMoneyMethodName = "AddMoney";

    [Header("Animation")]
    [SerializeField] private float punchScale = 1.35f;
    [SerializeField] private float punchTime = 0.09f;
    [SerializeField] private float returnTime = 0.12f;
    [SerializeField] private float popupTime = 0.55f;
    [SerializeField] private float popupJumpY = 35f;

    private int currentCombo;
    private int localMoney;

    private Vector3 comboBaseScale = Vector3.one;
    private Vector3 moneyBaseScale = Vector3.one;
    private Vector3 popupBaseScale = Vector3.one;
    private Vector2 popupBasePosition;

    private Coroutine comboPunchRoutine;
    private Coroutine moneyPunchRoutine;
    private Coroutine popupRoutine;

    public int CurrentCombo => currentCombo;

    // Это только локальные деньги ComboSystem. В основной игре деньги лучше читать из GameSessionBridge.
    public int CurrentMoney => controlMoneyTextDirectly ? localMoney : (GameSessionBridge.Instance != null ? GameSessionBridge.Instance.CurrentMoney : localMoney);

    private void Awake()
    {
        Instance = this;
        localMoney = startMoney;
    }

    private void Start()
    {
        if (comboText != null) comboBaseScale = comboText.transform.localScale;

        if (moneyText != null) moneyBaseScale = moneyText.transform.localScale;

        if (moneyPopupText != null)
        {
            popupBaseScale = moneyPopupText.transform.localScale;
            popupBasePosition = moneyPopupText.rectTransform.anchoredPosition;
            moneyPopupText.gameObject.SetActive(false);
        }

        UpdateComboText();

        
    }

    private void OnEnable()
    {
        DayController.dayEnd += OnDayEvent;
    }

    private void OnDisable()
    {
        DayController.dayEnd -= OnDayEvent;
    }

    private void OnDayEvent(bool ended)
    {
        ResetCombo();
    }

    public void RegisterFlyerResult(bool isCorrect)
    {
        RegisterFlyerResult(isCorrect, 1);
    }

    public void RegisterFlyerResult(bool isCorrect, int correctAmount)
    {
        if (isCorrect)
        {
            AddComboAmount(correctAmount);
        }         
        else ResetCombo();
    }

    public void AddCombo()
    {
        AddComboAmount(1);
    }

    public void AddComboAmount(int amount)
    {
        if (amount <= 0) return;

        int oldCombo = currentCombo;
        currentCombo += amount;
        UpdateComboText();
        PunchComboText();

        int moneyReward = 0;
        for (int comboValue = oldCombo + 1; comboValue <= currentCombo; comboValue++)
        {
            moneyReward += GetRewardForCombo(comboValue);
        }         
        if (moneyReward > 0) AddMoney(moneyReward);

    }

    public void ResetCombo()
    {
        currentCombo = 0;
        UpdateComboText();
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;

        bool sentToGameMoney = TrySendMoneyToReceiver(amount);

        if (controlMoneyTextDirectly || !sentToGameMoney) localMoney += amount;

        PunchMoneyText();
        ShowMoneyPopup(amount);
    }

    private int GetRewardForCombo(int combo)
    {
        if (rewards == null) return 0;

        for (int i = 0; i < rewards.Length; i++)
        {
            if (rewards[i] != null && rewards[i].combo == combo) return rewards[i].money;
        }
        return 0;
    }

    private void UpdateComboText()
    {
        if (comboText != null) comboText.text = comboPrefix + currentCombo;
    }

    private void PunchComboText()
    {
        if (comboText == null) return;
        if (comboPunchRoutine != null) StopCoroutine(comboPunchRoutine);

        comboPunchRoutine = StartCoroutine(PunchRoutine(comboText.transform, comboBaseScale));
    }

    private void PunchMoneyText()
    {
        if (moneyText == null) return;
        if (moneyPunchRoutine != null) StopCoroutine(moneyPunchRoutine);

        moneyPunchRoutine = StartCoroutine(PunchRoutine(moneyText.transform, moneyBaseScale));
    }

    private IEnumerator PunchRoutine(Transform target, Vector3 baseScale)
    {
        if (target == null) yield break;

        float timer = 0f;
        Vector3 bigScale = baseScale * punchScale;

        while (timer < punchTime)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / punchTime);
            target.localScale = Vector3.Lerp(baseScale, bigScale, t);
            yield return null;
        }

        timer = 0f;
        while (timer < returnTime)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / returnTime);
            target.localScale = Vector3.Lerp(bigScale, baseScale, t);
            yield return null;
        }

        target.localScale = baseScale;
    }

    private void ShowMoneyPopup(int amount)
    {
        if (moneyPopupText == null) return;
        if (popupRoutine != null) StopCoroutine(popupRoutine);

        popupRoutine = StartCoroutine(MoneyPopupRoutine(amount));
    }

    private IEnumerator MoneyPopupRoutine(int amount)
    {
        moneyPopupText.gameObject.SetActive(true);
        moneyPopupText.text = "+$" + amount;
        moneyPopupText.transform.localScale = popupBaseScale;
        moneyPopupText.rectTransform.anchoredPosition = popupBasePosition;

        Color baseColor = moneyPopupText.color;
        Color visibleColor = baseColor;
        visibleColor.a = 1f;
        moneyPopupText.color = visibleColor;

        float timer = 0f;
        while (timer < popupTime)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / popupTime);

            moneyPopupText.rectTransform.anchoredPosition = popupBasePosition + new Vector2(0f, popupJumpY * t);
            moneyPopupText.transform.localScale = Vector3.Lerp(popupBaseScale * punchScale, popupBaseScale, t);

            Color color = visibleColor;
            color.a = Mathf.Lerp(1f, 0f, t);
            moneyPopupText.color = color;

            yield return null;
        }

        moneyPopupText.rectTransform.anchoredPosition = popupBasePosition;
        moneyPopupText.transform.localScale = popupBaseScale;
        moneyPopupText.color = baseColor;
        moneyPopupText.gameObject.SetActive(false);
    }

    private bool TrySendMoneyToReceiver(int amount)
    {
        if (moneyReceiver != null) return TryInvokeAddMoney(moneyReceiver, amount);

        if (GameSessionBridge.Instance != null)
        {
            GameSessionBridge.Instance.Money += amount;
            return true;
        }

        return false;
    }

    private bool TryInvokeAddMoney(MonoBehaviour receiver, int amount)
    {
        MethodInfo method = receiver.GetType().GetMethod(
            addMoneyMethodName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new Type[] { typeof(int) },
            null
        );

        if (method == null) return false;


        method.Invoke(receiver, new object[] { amount });
        return true;
    }
}
