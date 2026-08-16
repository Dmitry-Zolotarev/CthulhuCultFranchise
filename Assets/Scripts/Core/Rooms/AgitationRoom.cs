using TMPro;
using UnityEngine;

public class AgitationRoom : Room
{
    [SerializeField] private float agitationSpeed = 1;

    private float agitationProgress = 0;
    [HideInInspector] public int agitationTargetValue = 100;

    [SerializeField] private TextMeshProUGUI districtNameLabel;
    [SerializeField] private TextMeshProUGUI progressLabel;
    
    void Awake()
    {
        roomType = RoomType.Agitation;
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
        if (agitationProgress == agitationTargetValue * district.Influence)
        {
            district.Influence++;
            agitationProgress = 0;
        }
        
    }
    private int GetProgressPercent()
    {
        return ((int)agitationProgress * 100 / agitationTargetValue / GameManager.Instance.SelectedDistrict.Influence) % 100;
    }
    private void UpdateLabels()
    {
        districtNameLabel.SetText($"{GameManager.Instance.SelectedDistrict.Name}");
        progressLabel.SetText($"Прогресс: {GetProgressPercent()}%");
    }
}
