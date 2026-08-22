using TMPro;
using UnityEngine;

public class AgitationRoom : Room
{
    [SerializeField] private float agitationSpeed = 1;

    private float agitationProgress = 0;
    private float agitationTargetValue = 100;

    [SerializeField] private TextMeshProUGUI districtNameLabel;
    [SerializeField] private TextMeshProUGUI progressLabel;
    private District district;


    private void Update()
    {
        if (GameManager.Instance.phase != GamePhase.Office) return;

        if (district != null && GameManager.Instance.SelectedDistrict != district)  
        {
            agitationProgress = 0;
        }
        district = GameManager.Instance.SelectedDistrict;
        agitationTargetValue = 100 * district.Influence;

        districtNameLabel?.SetText($"{district.Name}: {district.Influence}/5");
        progressLabel?.SetText($"Прогресс: {GetProgressPercent()}%");

        if (district.Influence == 5) 
        {
            progressLabel?.SetText("Агитация завершена");
            return;
        } 
        agitationProgress += agitationSpeed * GetCurrentPersonCount() * Time.deltaTime * GameManager.Instance.GetTimeSpeed();
        if (agitationProgress >= agitationTargetValue)
        {
            district.Influence++;
            agitationProgress = 0;
        }
        
    }
    private int GetProgressPercent()
    {
        float percent = agitationProgress * 100 / agitationTargetValue;
        return (int)percent;
    }
}
