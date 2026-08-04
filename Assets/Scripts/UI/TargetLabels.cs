using UnityEngine;
using TMPro;

public class TargetLabels : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI needAdepts;
    [SerializeField] private TextMeshProUGUI needMoney;

    private void Start()
    {
        try
        {
            needAdepts?.SetText($"Привлечь {Messa.Instance.NeedAdepts} адептов");
            needMoney?.SetText($"Заработать ${Messa.Instance.NeedMoney}");
        }
        catch { }       
    }
}
