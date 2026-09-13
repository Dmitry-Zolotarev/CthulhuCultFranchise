using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Header;
    [SerializeField] private TextMeshProUGUI MoneyLabel;
    [SerializeField] private TextMeshProUGUI DistrictLabel;
    [SerializeField] private TextMeshProUGUI InfluenceLabel;
    [SerializeField] private TextMeshProUGUI EvidenceLabel;

    void OnEnable()
    {
        Header?.SetText($"День {GameManager.Instance.Day} окончен");
        MoneyLabel?.SetText($"+{GameManager.Instance.TodayEarned}$");
        DistrictLabel?.SetText(GameManager.Instance.SelectedDistrict.Name);
        InfluenceLabel?.SetText($"+{GameManager.Instance.TodayInfluence} влияния");
        EvidenceLabel?.SetText($"+{GameManager.Instance.TodayEvidences} улик{PluralEnding.GetEnding(GameManager.Instance.TodayEvidences)}");
    }
}
