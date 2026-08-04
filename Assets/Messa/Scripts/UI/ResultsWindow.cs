using UnityEngine;
using TMPro;

public class ResultsWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mainResultLabel;
    [SerializeField] private TextMeshProUGUI visitorsAmountLabel;
    [SerializeField] private TextMeshProUGUI newAdeptsLabel;
    [SerializeField] private TextMeshProUGUI adeptsOutflowLabel;
    [SerializeField] private TextMeshProUGUI dailyIncomeLabel;
    [SerializeField] private TextMeshProUGUI[] visitorsCountLabels;
    [SerializeField] private TextMeshProUGUI[] newAdeptsCountLabels;
    [SerializeField] private TextMeshProUGUI[] MoneyIncomeLabels;

    private Color goodResultColor;
  
    private void Start()
    {
        goodResultColor = mainResultLabel.color;
        UpdateUI();
    }
    private void OnEnable()
    {
        UpdateUI();
    }
    private void UpdateUI()
    {
        if(Messa.Instance.ResultText.Contains("Плохая"))
        {
            var color = goodResultColor;
            color.r = goodResultColor.g;
            color.g = goodResultColor.r;
            mainResultLabel.color = color;
        }
        else mainResultLabel.color = goodResultColor;

        mainResultLabel?.SetText(Messa.Instance.ResultText);
        visitorsAmountLabel?.SetText(Messa.Instance.GetVisitorsCount());
        newAdeptsLabel?.SetText(Messa.Instance.GetNewAdeptsCount());
        adeptsOutflowLabel?.SetText(Messa.Instance.GetAdeptsOutflow());


        for (int i = 0; i < 5; i++)
        {
            try
            {
                visitorsCountLabels[i]?.SetText($"Пришло: {Messa.Instance.Auditory[i]}");
                newAdeptsCountLabels[i]?.SetText($"Стали адептами: {Messa.Instance.NewAdepts[i]}");
                MoneyIncomeLabels[i]?.SetText($"${Messa.Instance.DailyMoneyIncomes[i]}");
            }
            catch { }          
        }
        dailyIncomeLabel?.SetText($"${(int)Messa.Instance.DailyMoneyIncome}");
    }
}