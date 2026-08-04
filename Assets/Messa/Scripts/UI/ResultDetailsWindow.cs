using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultDetailsWindow : MonoBehaviour
{
    

    [SerializeField] private TextMeshProUGUI visitorsAmountLabel;
    [SerializeField] private TextMeshProUGUI newAdeptsLabel;
    [SerializeField] private TextMeshProUGUI dailyIncomeLabel;

    [SerializeField] private TextMeshProUGUI baseIncomeLabel;
    [SerializeField] private TextMeshProUGUI totalIncomeLabel;
    [SerializeField] private TextMeshProUGUI conversionChanceLabel;

    [Header("Тип посетителей")]
    [SerializeField] private TextMeshProUGUI visitorTypeLabel1;
    [SerializeField] private string[] visitorTypeStrings1;
    [SerializeField] private TextMeshProUGUI visitorTypeLabel2;
    [SerializeField] private string[] visitorTypeStrings2;

    [Header("Бонусы к конверсии")]
    [SerializeField] private GameObject cookieBonus;
    [SerializeField] private GameObject premiumFlyerConversionBonus;
    [SerializeField] private GameObject beautifulAltarBonus;

    [SerializeField] private TextMeshProUGUI baseChanceLabel;
    [SerializeField] private TextMeshProUGUI sermonMultiplierLabel;
    [SerializeField] private TextMeshProUGUI esotericBonusLabel;
    [SerializeField] private TextMeshProUGUI cookieBonusLabel;
    [SerializeField] private TextMeshProUGUI flyerConversionBonusLabel;
    [SerializeField] private TextMeshProUGUI altarBonusLabel;

    [Header("Бонусы к доходу")]
    [SerializeField] private GameObject firstRowBonus;
    [SerializeField] private GameObject premiumCandleBonus;
    [SerializeField] private GameObject premiumFlyerMoneyBonus;

    [SerializeField] private TextMeshProUGUI bonusesHeader;
    [SerializeField] private TextMeshProUGUI firstRowBonusLabel;
    [SerializeField] private TextMeshProUGUI premiumCandleBonusLabel;
    [SerializeField] private TextMeshProUGUI premiumFlyerBonusLabel;    

    [Header("Комментарий Ктулху")]
    [SerializeField] private TextMeshProUGUI cthulhuCommentLabel;
    [TextArea][SerializeField] private string[] cthulhuComments;

    [Header("Аватары посетителей")]
    [SerializeField] private Image VisitorAvatar;
    [SerializeField] private Sprite[] VisitorAvatarSprites;

    private int currentIndex;
    public void UpdateData(int i)
    {
        if (i > 4) return;

        currentIndex = i;

        visitorTypeLabel1?.SetText("Подробно: " + visitorTypeStrings1[i].ToLower());
        visitorTypeLabel2?.SetText("Итог по " + visitorTypeStrings2[i].ToLower());
        VisitorAvatar.sprite = VisitorAvatarSprites[i];

        visitorsAmountLabel?.SetText($"{Messa.Instance.Auditory[i]}");
        newAdeptsLabel?.SetText($"{Messa.Instance.NewAdepts[i]}");
        dailyIncomeLabel?.SetText($"${Messa.Instance.DailyMoneyIncomes[i]}");
        cthulhuCommentLabel?.SetText(cthulhuComments[i]);

        float dailyBaseIncome = Messa.Instance.NewAdepts[i] * Messa.Instance.BaseIncome[i];

        baseIncomeLabel?.SetText($"{Messa.Instance.NewAdepts[i]} * ${Messa.Instance.BaseIncome[i]} = {dailyBaseIncome}");
        totalIncomeLabel?.SetText($"Итоговый доход: ${Messa.Instance.DailyMoneyIncomes[i]}");

        bonusesHeader.SetText("Бонусов нет");

        //Бонусы к конверсии
        baseChanceLabel?.SetText($"{Messa.Instance.BaseConversion[i] * 100}%");     
        sermonMultiplierLabel?.SetText($"x{Messa.Instance.ConversionMultiplier}");
        esotericBonusLabel?.SetText($"+{Messa.Instance.EsotericBonus * 100}%");

        conversionChanceLabel?.SetText($"{(int)(Messa.Instance.ConversionChances[i] * 100)}%");

        if (i == 1 && Messa.Instance.IsUnlocked(Upgrades.CookiesAfterMessa))
        {
            cookieBonus?.SetActive(true);
            cookieBonusLabel?.SetText($"+{Messa.Instance.CookieBonus * 100}%");
        }
        else cookieBonus?.SetActive(false);

        if (Messa.Instance.IsUnlocked(Upgrades.PremiumFlyer))
        {
            premiumFlyerConversionBonus?.SetActive(true);
            flyerConversionBonusLabel?.SetText($"+{Messa.Instance.PremiumFlyerConversionBonus * 100}%");
        }
        else premiumFlyerConversionBonus.SetActive(false);

        if (i == 0 && Messa.Instance.IsUnlocked(Upgrades.BeautifulAltar))
        {
            beautifulAltarBonus?.SetActive(true);
            altarBonusLabel?.SetText($"x{Messa.Instance.AltarBonus * 100}%");
        }
        else beautifulAltarBonus?.SetActive(false);

        //Бонусы к доходу
        if (i == 0 && Messa.Instance.IsUnlocked(Upgrades.PaidFrontRow))
        {
            firstRowBonus?.SetActive(true);
            firstRowBonusLabel?.SetText($"+${Messa.Instance.GetFrontRowBonus()}");
            bonusesHeader.SetText("Бонусы");
        }
        else firstRowBonus.SetActive(false);

        if (Messa.Instance.IsUnlocked(Upgrades.PremiumCandles))
        {
            premiumCandleBonus?.SetActive(true);
            premiumCandleBonusLabel?.SetText($"x{Messa.Instance.PremiumCandlesMultiplier}");
            bonusesHeader.SetText("Бонусы");
        }
        else premiumCandleBonus?.SetActive(false);

        if (Messa.Instance.IsUnlocked(Upgrades.PremiumFlyer))
        {
            premiumFlyerMoneyBonus?.SetActive(true);
            premiumFlyerBonusLabel?.SetText($"x{Messa.Instance.PremiumFlyerMoneyBonus}");
            bonusesHeader.SetText("Бонусы");
        }
        else premiumFlyerMoneyBonus?.SetActive(false);    
    }  
    public void Next()
    {
        if (currentIndex < 4)
        {
            UpdateData(currentIndex + 1);
        }
        else Messa.Instance.OpenMenu(1);
    }
}
