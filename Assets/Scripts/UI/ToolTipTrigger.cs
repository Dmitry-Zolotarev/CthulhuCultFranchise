using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] public string Text;
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private float fontSize = 16f;
    
    public void OnPointerEnter(PointerEventData eventData) => ShowTooltip(Text);
    public void OnPointerExit(PointerEventData eventData) => ToolTip.Instance.HideTooltip();
    public void ShowTooltip(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        ToolTip.Instance.transform.position = transform.position + offset;
        ToolTip.Instance.SetFontSize(fontSize);
        ToolTip.Instance.ShowTooltip(text);
    }
    public void ShowTooltip()
    {
        if (string.IsNullOrEmpty(Text)) return;

        ToolTip.Instance.transform.position = transform.position + offset;
        ToolTip.Instance.SetFontSize(fontSize);
        ToolTip.Instance.ShowTooltip(Text);
    }
    public void HideToolTip() => ToolTip.Instance.HideTooltip();
}