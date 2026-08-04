using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private Image ResultImage;
    [SerializeField] private Sprite GoodResult;
    [SerializeField] private Sprite BadResult;
    [SerializeField] private TextMeshProUGUI adeptsLabel;
    [SerializeField] private TextMeshProUGUI needAdeptsLabel;
    [SerializeField] private TextMeshProUGUI moneyLabel;
    [SerializeField] private TextMeshProUGUI needMoneyLabel;
    [SerializeField] private TextMeshProUGUI adeptsPlanLabel;
    [SerializeField] private TextMeshProUGUI moneyPlanLabel;

    void Start()
    {
        UpdateUI();
    }
    void OnEnable()
    {
        UpdateUI();
    }
    void UpdateUI()
    {
        adeptsLabel.color = new Color(0.3f, 0.2f, 0.3f);
        moneyLabel.color = new Color(0.4f, 0.5f, 0.2f);

        if (Messa.Instance.IsGoodGameResult())
        {
            ResultImage.sprite = GoodResult;
        }
        else ResultImage.sprite = BadResult;

        if (Messa.Instance.GetTotalAdeptsCount() > Messa.Instance.NeedAdepts)
        {
            adeptsPlanLabel.SetText($"+{Messa.Instance.GetTotalAdeptsCount() - Messa.Instance.NeedAdepts} сверх плана!");
            adeptsPlanLabel.color = adeptsLabel.color = new Color(0f, 0.5f, 0f);
        }
        else if (Messa.Instance.GetTotalAdeptsCount() < Messa.Instance.NeedAdepts)
        {
            
            adeptsPlanLabel.SetText($"-{Messa.Instance.NeedAdepts - Messa.Instance.GetTotalAdeptsCount()} до плана!");
            adeptsPlanLabel.color = adeptsLabel.color = new Color(0.5f, 0f, 0f);
            adeptsLabel.color = new Color(0.5f, 0f, 0f);
        }
        else adeptsPlanLabel.SetText("");

        if ((int)Messa.Instance.Money > Messa.Instance.NeedMoney)
        {
            moneyPlanLabel.SetText($"+${(int)Messa.Instance.Money - Messa.Instance.NeedMoney} сверх плана!");
            moneyPlanLabel.color = moneyPlanLabel.color = new Color(0f, 0.5f, 0f);
        }
        else if ((int)Messa.Instance.Money < Messa.Instance.NeedMoney)
        {
            moneyPlanLabel.SetText($"-${Messa.Instance.NeedMoney - (int)Messa.Instance.Money} до плана!");
            moneyPlanLabel.color = moneyPlanLabel.color = new Color(0.5f, 0f, 0f);
            moneyLabel.color = new Color(0.5f, 0f, 0f);
        }
        else moneyPlanLabel.SetText("");
        
        adeptsLabel?.SetText($"{Messa.Instance.GetTotalAdeptsCount()}");
        needAdeptsLabel?.SetText($"{Messa.Instance.NeedAdepts}");
        moneyLabel?.SetText($"${(int)Messa.Instance.Money}");
        needMoneyLabel?.SetText($"${Messa.Instance.NeedMoney}");
    }
}
