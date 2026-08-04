using UnityEngine;
using TMPro;
public class StreetResultsWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI visitorsCount;
    [SerializeField] private TextMeshProUGUI[] visitorTypeLabels;
    [SerializeField] private TextMeshProUGUI cthulhuComment;
    public void UpdateLabels(int[] visitorCounts)
    {
        int totalCount = Messa.Instance.TotalCount(visitorCounts);
        visitorsCount?.SetText($"{totalCount}");
        
        for(int i = 0; i < visitorTypeLabels.Length; i++)
        {
            visitorTypeLabels[i]?.SetText($"x{visitorCounts[i]}");
        }
        if (totalCount > 0) 
        {
            cthulhuComment?.SetText("Хорошая работа!\nЛюди идут.");
        } 
        else cthulhuComment?.SetText("Плохо старался!\nНикто не пришел.");
    }
}
