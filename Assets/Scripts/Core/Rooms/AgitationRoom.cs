using TMPro;
using UnityEngine;

public class AgitationRoom : Room
{
    [SerializeField] private float agitationSpeed = 1;

    private float agitationProgress = 0;
    private float agitationTargetValue;

    [SerializeField] private TextMeshProUGUI districtNameLabel;
    [SerializeField] private TextMeshProUGUI progressLabel;
    
    void Awake()
    {
        roomType = RoomType.Agitation;
        agitationTargetValue = 100;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!OfficeManager.Instance.ShiftRunning) return;

        var district = GameManager.Instance.SelectedDistrict;
        UpdateLabels();

        if (district.Influence == 5) 
        {
            progressLabel.SetText("Агитация завершена");
            return;
        } 
        agitationProgress += agitationSpeed * GetCurrentPersonCount() * Time.deltaTime * OfficeManager.Instance.GetTimeSpeed();
        if (agitationProgress >= agitationTargetValue)
        {
            district.Influence++;
            agitationTargetValue = 100 * district.Influence;
            Debug.Log($"{district.Name}: {district.Influence}/5");
            agitationProgress = 0;
        }
        
    }
    private int GetProgressPercent()
    {
        float percent = agitationProgress * 100 / agitationTargetValue;
        return (int)percent;
    }
    private void UpdateLabels()
    {
        var district = GameManager.Instance.SelectedDistrict;

        districtNameLabel.SetText($"{district.Name}: {district.Influence}/5");
        progressLabel.SetText($"Прогресс: {GetProgressPercent()}%");
    }
}
