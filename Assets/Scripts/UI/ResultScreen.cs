using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI moneyLabel;
    [SerializeField] private TextMeshProUGUI taxLabel;
    [SerializeField] private TextMeshProUGUI districtLabel;
    [SerializeField] private TextMeshProUGUI influenceLabel;
    [SerializeField] private TextMeshProUGUI evidenceLabel;
    [SerializeField] private TextMeshProUGUI escapeLabel;
    [SerializeField] private TextMeshProUGUI victimLabel;
    [SerializeField] private Slider suspicionBar;
    void OnEnable()
    {
        var game = GameManager.Instance;
        header?.SetText($"День {game.Day} окончен");
        moneyLabel?.SetText($"+{game.TodayEarned}$");
        taxLabel?.SetText($"Налог: {TaxManager.Instance.TaxAmount}$");
        districtLabel?.SetText(game.SelectedDistrict.Name);
        influenceLabel?.SetText($"Влияние: {game.SelectedDistrict.Influence} / 5 (+{game.TodayInfluence})");     
        evidenceLabel?.SetText($"Найдено улик: {game.TodayEvidences}");
        escapeLabel?.SetText($"Сбежало культистов: {game.TodayEscaped}");
        victimLabel?.SetText($"Съедено культистов: {game.TodayEaten}");
        suspicionBar.value = game.EvidencesCount / game.MaxSuspicion;
    }
}
