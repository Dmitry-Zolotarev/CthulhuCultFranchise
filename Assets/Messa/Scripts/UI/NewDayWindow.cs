using UnityEngine;
using TMPro;


public class NewDayWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NewDayLabel;
    [SerializeField] private TextMeshProUGUI ButtonLabel;

    private void OnEnable()
    {
        int newDay = Messa.Instance.CurrentDay + 1;
        NewDayLabel.SetText($"Наступает день {newDay}...");
        ButtonLabel.SetText($"Начать день {newDay}");
    }
}
