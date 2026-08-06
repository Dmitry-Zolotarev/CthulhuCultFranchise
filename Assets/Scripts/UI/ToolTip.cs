using System.Collections;
using TMPro;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private GameObject toolTipImage;
    [SerializeField] private int SecondsBeforeClose = 2;
    public static ToolTip Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public void ShowTooltip(string text)
    {
        toolTipImage.SetActive(true);
        tooltipText.text = text;
        if(SecondsBeforeClose > 0) StartCoroutine(HideTooltipCoroutine());
    }
    public void HideTooltip()
    {
        toolTipImage.SetActive(false);
    }
    public void SetFontSize(float value) => tooltipText.fontSize = value;

    private IEnumerator HideTooltipCoroutine() 
    {
        yield return new WaitForSeconds(SecondsBeforeClose);
        HideTooltip();
    }
}