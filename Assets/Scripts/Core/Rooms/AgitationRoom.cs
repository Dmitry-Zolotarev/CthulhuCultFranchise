using TMPro;
using UnityEngine;

public class AgitationRoom : Room
{
    [SerializeField] private float agitationSpeed = 1;

    
    private float agitationTargetValue = 100;

    [SerializeField] private TextMeshProUGUI districtNameLabel;
    [SerializeField] private TextMeshProUGUI progressLabel;
    private District district;


    private void Update()
    {
        if (GameManager.Instance.Phase != GamePhase.Office) return;

        var agitationProgress = GameManager.Instance.AgitationProgress;

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
        agitationProgress += agitationSpeed * Level * GetCurrentPersonCount() * Time.deltaTime * GameManager.Instance.GetTimeSpeed();
        if (agitationProgress >= agitationTargetValue)
        {
            district.Influence++;
            agitationProgress = 0;
        }
        GameManager.Instance.AgitationProgress = agitationProgress;
    }
    private int GetProgressPercent()
    {
        float percent = GameManager.Instance.AgitationProgress * 100 / agitationTargetValue;
        return (int)percent;
    }
}
