using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class RowIcon : MonoBehaviour
{
    private TextMeshProUGUI textLabel;
    private Image image;
    void Awake()
    {
        textLabel = GetComponentInParent<TextMeshProUGUI>();
        image = GetComponent<Image>();
    }
    private void Update()
    {
        var color = image.color;
        color.a = string.IsNullOrEmpty(textLabel.text) ? 0f : 1f;
        image.color = color;
    }
}
