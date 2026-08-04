using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class Messa : MonoBehaviour
{
    public int oldAdeptsCount;
    [Header("Основные параметры")]
    
    public float messaDuration = 6f;
    public float Money;
    [HideInInspector] public float DailyMoneyIncome;
    [HideInInspector] public float[] DailyMoneyIncomes = new float[5];
    [HideInInspector] public int CurrentDay = 1;

    [HideInInspector] public int[] Auditory = new int[5];
    [HideInInspector] public int[] NewAdepts = new int[5];
    [HideInInspector] public int[] OldAdepts = new int[5];
    [HideInInspector] public string ResultText = "Отличная месса!";

    [Header("Базовая конверсия")]
    public float[] BaseConversion = new float[5] { 0.30f, 0.52f, 0.34f, 0.30f, 0.38f };
    public float ConversionMultiplier;

    [Header("Базовый доход")]
    public int[] BaseIncome = new int[5] { 10, 2, 3, 5, 3 };

    [Header("Целевые значения")]
    public int NeedAdepts = 15;
    public int NeedMoney = 50;


    [Header("Улучшения")]
    public UpgradeInfo[] UpgradeList;
    public UpgradePanel UpgradePanel1;
    public UpgradePanel UpgradePanel2;
    public UpgradePanel UpgradePanel3;
    private List<int> purchasedUpgrades = new List<int>();
    public List<UpgradePanel> purchasedUpgradesPanels;
    public GameObject purchasedUpgradesBigPanel;

    [Header("Множители проповеди")]
    public float BadMultiplier = 0.68f;
    public float NormalMultiplier = 1.0f;
    public float GoodMultiplier = 1.22f;
    public float ExcellentMultiplier = 1.35f;

    [Header("Пороги")]
    public float BadThreshold = 0.18f;
    public float GoodThreshold = 0.34f;
    public float ExcellentThreshold = 0.50f;

    [Header("Бонусы к конверсии")]
    public float CookieBonus = 0.1f;
    public float AltarBonus = 0.08f;
    public float PremiumFlyerConversionBonus = 0.05f;
    public float EsotericBonusPerUnit = 0.02f;
    public float EsotericBonus = 0f;

    [HideInInspector] public float[] ConversionChances = new float[5];

    [Header("Бонусы к доходу")]
    public int PaidFrontRowBonusPerUnit = 1;
    public float PremiumCandlesMultiplier = 1.20f;
    public float PremiumFlyerMoneyBonus = 1.25f;
    public int AbyssAccountantBonus = 20;

    [Header("Лимиты")]
    public float MaxConversionChance = 0.90f;

    [Header("Отток")]
    public float BaseChurn = 0.08f;
    public float PensionerChurnReduce = 0.01f;
    public float MinChurn = 0.02f;
    public float ChoirMultiplier = 0.5f;


    [Header("UI")]
    public GameObject StatsPanel;
    public GameObject UpgradeWindow;
    public TextMeshProUGUI DayLabel;
    public TextMeshProUGUI MoneyLabel;
    public TextMeshProUGUI OldAdeptsCountLabel;

    public TextMeshProUGUI AuditoryWorkersLabel;
    public TextMeshProUGUI AuditoryStudentsLabel;
    public TextMeshProUGUI AuditoryPensionersLabel;
    public TextMeshProUGUI AuditoryBloggersLabel;
    public TextMeshProUGUI AuditoryEsotericsLabel;
    
    public GameObject[] Menus;
    [Header("Спрайты посетителей")]
    [SerializeField] private GameObject[] VisitorSprites;
  
    public static Messa Instance;
    private void Awake()
    {
        Instance = this;
        foreach (var panel in purchasedUpgradesPanels) panel.gameObject.SetActive(false);
        purchasedUpgradesBigPanel?.SetActive(false);
    }
    private void Start()
    {
        LoadFromBridge();
    }
    private void OnEnable()
    {
        LoadFromBridge();
    }
    private void LoadFromBridge()
    {    
        if (GameSessionBridge.Instance == null) return;
        var input = GameSessionBridge.Instance.GetMessaInput();

        CurrentDay = input.currentDay;
        Money = input.currentMoney;

        Auditory[0] = input.officeVisitors;
        Auditory[1] = input.studentVisitors;
        Auditory[2] = input.retireeVisitors;
        Auditory[3] = input.bloggerVisitors;
        Auditory[4] = input.esotericVisitors;

        OpenMenu((int)MenuID.MessaHall);
        UpdateUI();
    }
    
    private void OpenMenu(MenuID menuID)
    {
        for (int i = 0; i < Menus.Length; i++) Menus[i]?.SetActive(i == (int)menuID);
        StatsPanel.SetActive(menuID != MenuID.PendingMessa);
    }
    public void OpenMenu(int menuID)
    {
        SFXPlayer.Instance.Play("Клик");
        for (int i = 0; i < Menus.Length; i++) Menus[i]?.SetActive(i == menuID);
        StatsPanel.SetActive((MenuID)menuID != MenuID.PendingMessa);
    }
    public void UpdateUI()
    {
        DayLabel?.SetText($"{CurrentDay}");
        MoneyLabel?.SetText($"${(int)Money}");
        OldAdeptsCountLabel?.SetText($"Старые адепты: {TotalCount(OldAdepts)}");

        AuditoryWorkersLabel?.SetText($"x{Auditory[0]}");
        AuditoryStudentsLabel?.SetText($"x{Auditory[1]}");
        AuditoryPensionersLabel?.SetText($"x{Auditory[2]}");
        AuditoryBloggersLabel?.SetText($"x{Auditory[3]}");
        AuditoryEsotericsLabel?.SetText($"x{Auditory[4]}");

        for(int i = 0; i < VisitorSprites.Length; i++)
        {
            VisitorSprites[i].SetActive(Auditory[i] > 0 || OldAdepts[i] > 0);
        }
    }
    public int TotalCount(int[] array)
    {
        int count = 0;
        foreach (int i in array) count += i;
        return count;
    }
    public int GetOldAdeptsCount()
    {
        return TotalCount(OldAdepts);
    }
    public string GetNewAdeptsCount()
    {
        return TotalCount(NewAdepts).ToString();
    }
    public string GetVisitorsCount()
    {
        return TotalCount(Auditory).ToString();
    }
    public int GetTotalAdeptsCount()
    {
        return TotalCount(OldAdepts) + TotalCount(NewAdepts);
    }
    public string GetAdeptsOutflow()
    {
        return $"{oldAdeptsCount - TotalCount(OldAdepts)}";
    }
    public int GetFrontRowBonus()
    {
        return IsUnlocked(Upgrades.PaidFrontRow) ? Auditory[0] * PaidFrontRowBonusPerUnit : 0;
    }
    public void BuyUpgrade(int i)
    {
        if (Money < UpgradeList[i].Price || UpgradeList[i].Unlocked || i >= UpgradeList.Length)
        {
            SFXPlayer.Instance.Play("Клик");
            return;
        } 
        purchasedUpgradesBigPanel.SetActive(true);     
        Money -= UpgradeList[i].Price;
        UpgradeList[i].Unlocked = true;
        SFXPlayer.Instance.Play("Покупка");
        purchasedUpgrades.Add(i);
        purchasedUpgradesPanels[purchasedUpgrades.Count - 1].gameObject.SetActive(true);
        purchasedUpgradesPanels[purchasedUpgrades.Count - 1].BindUpgrade(i);
        Next();
    }
    public bool IsUnlocked(Upgrades upgrade)
    {
        return UpgradeList[(int)upgrade].Unlocked;
    }
    public void SpellSermon(int peopleClass)
    {
        oldAdeptsCount = TotalCount(OldAdepts);
        int totalVisitors = TotalCount(Auditory);

        float share = (float)Auditory[peopleClass] / totalVisitors;

        bool isBad = false;
        bool isGoodOrBetter = false;
        bool isExcellent = false;

        if (share < BadThreshold)
        {
            ConversionMultiplier = BadMultiplier;
            isBad = true;
        }
        else if (share < GoodThreshold)
        {
            ConversionMultiplier = NormalMultiplier;
        }
        else if (share < ExcellentThreshold)
        {
            ConversionMultiplier = GoodMultiplier;
            isGoodOrBetter = true;
        }
        else
        {
            ConversionMultiplier = ExcellentMultiplier;
            isGoodOrBetter = true;
            isExcellent = true;
        }
        EsotericBonus = Auditory[4] * EsotericBonusPerUnit;

        for (int i = 0; i < 5; i++)
        {
            float chance = BaseConversion[i] * ConversionMultiplier;

            if (IsUnlocked(Upgrades.CookiesAfterMessa) && i == 1) chance += CookieBonus;

            if (IsUnlocked(Upgrades.BeautifulAltar)) chance += AltarBonus;
            chance += EsotericBonus;

            if (IsUnlocked(Upgrades.PremiumFlyer)) chance += PremiumFlyerConversionBonus;

            ConversionChances[i] = Mathf.Min(chance, MaxConversionChance);
            NewAdepts[i] = Mathf.RoundToInt(Auditory[i] * ConversionChances[i]);
        }
        int lostTotal = 0;
        if (isBad)
        {
            float churn = BaseChurn - Auditory[2] * PensionerChurnReduce;
            churn = Mathf.Max(churn, MinChurn);

            if (IsUnlocked(Upgrades.Choir)) churn *= ChoirMultiplier;

            for (int i = 0; i < 5; i++)
            {
                int lost = Mathf.RoundToInt(OldAdepts[i] * churn);
                OldAdepts[i] = OldAdepts[i] - lost;
                lostTotal += lost;
            }
        }
        DailyMoneyIncome = 0f;

        for (int i = 0; i < 5; i++)
        {
            DailyMoneyIncomes[i] = BaseIncome[i] * NewAdepts[i];
            if (IsUnlocked(Upgrades.PremiumFlyer)) DailyMoneyIncomes[i] *= PremiumFlyerMoneyBonus;
            if (IsUnlocked(Upgrades.PremiumCandles)) DailyMoneyIncomes[i] *= PremiumCandlesMultiplier;
            DailyMoneyIncome += DailyMoneyIncomes[i];
        }
        DailyMoneyIncome = DailyMoneyIncome + GetFrontRowBonus();

        if (IsUnlocked(Upgrades.PremiumCandles) && isGoodOrBetter) DailyMoneyIncome *= PremiumCandlesMultiplier;

        Money += DailyMoneyIncome;

        if (isExcellent)
        {
            ResultText = "Отличная месса!";
        }
        if (isGoodOrBetter)
        {
            ResultText = "Хорошая месса!";
        }
        if (isBad || TotalCount(Auditory) == 0)
        {
            ResultText = "Плохая месса!";
        }
        StartCoroutine(MessaCoroutine());
    }
    private IEnumerator MessaCoroutine()
    {
        SFXPlayer.Instance.Play("Месса");
        OpenMenu(MenuID.PendingMessa);
        SFXPlayer.Instance.Play("");    
        yield return new WaitForSeconds(messaDuration);
        UpdateUI();      
        OpenMenu(MenuID.MessaResults);
        SFXPlayer.Instance.Stop();
    }
    public void Next()
    {
        SFXPlayer.Instance.Play("Клик");
        UpdateUI();
        if (Menus[(int)MenuID.PendingMessa].activeSelf)
        {
            StopAllCoroutines();
            OpenMenu(MenuID.MessaResults);
            SFXPlayer.Instance.Stop();
        }
        else if (Menus[(int)MenuID.MessaResults].activeSelf || Menus[(int)MenuID.ResultDetails].activeSelf)
        {
            OpenMenu(MenuID.UpgradeShop);
            if (CurrentDay <= 4)
            {
                int j = (CurrentDay - 1) * 3;            
                UpgradePanel1.BindUpgrade(j);
                UpgradePanel2.BindUpgrade(j + 1);
                UpgradePanel3.BindUpgrade(j + 2);
            }
            else
            {
                UpgradeWindow?.SetActive(false);
            }
        }
        else if (Menus[(int)MenuID.UpgradeShop].activeSelf) 
        {
            UpgradeWindow?.SetActive(false);
        }       
    }
    public void StartNewDay()
    {
        InitiateAdepts();
        UpgradeWindow?.SetActive(true);

        if (CurrentDay < 5 && IsUnlocked(Upgrades.AbyssAccountant)) Money += AbyssAccountantBonus;

        GameSessionBridge.Instance.ApplyMessaResult(Money, TotalCount(OldAdepts));
        if (GameSessionBridge.Instance != null) GameSessionBridge.Instance.StartNewDay();
    }
    private void InitiateAdepts()
    {
        int n = Mathf.Min(OldAdepts.Length, NewAdepts.Length);
        for (int i = 0; i < n; i++)
        {
            OldAdepts[i] += NewAdepts[i];
            NewAdepts[i] = 0;
            Auditory[i] = 0;
        }
    }
    
    public void AddAdepts(int value)
    {
        if (value == 0) return;

        if (value > 0)
        {
            int perType = Mathf.Max(1, value / NewAdepts.Length);

            for (int i = 0; i < NewAdepts.Length; i++) NewAdepts[i] += perType;
        }
        else
        {
            int remaining = -value;
            int perType = Mathf.Max(1, remaining / OldAdepts.Length);

            for (int i = 0; i < OldAdepts.Length; i++)
            {
                OldAdepts[i] -= perType;
                if (OldAdepts[i] < 0) OldAdepts[i] = 0;
            }
        }
    }
    public bool IsGoodGameResult()
    {
        return Money >= NeedMoney && GetTotalAdeptsCount() >= NeedAdepts;
    }
}