using TMPro;
using UnityEngine;

public class MegafonAreaPreview : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer circleRenderer;
    [SerializeField] private TMP_Text bonusText;

    [Header("Text")]
    [SerializeField] private string bonusPrefix = "+";
    [SerializeField] private string bonusSuffix = " к мессе";
    [SerializeField] private string noBonusText = "+0";

    [Header("Colors")]
    [SerializeField] private Color activeColor = new Color(0.15f, 1f, 0.25f, 0.25f);
    [SerializeField] private Color emptyColor = new Color(0.15f, 1f, 0.25f, 0.10f);

    [Header("Scale")]
    [Tooltip("Если круговой sprite имеет диаметр 1 Unity unit, включи true. Тогда Scale = radius * 2.")]
    [SerializeField] private bool scaleByDiameter = true;
    [SerializeField] private float zOffset = 0f;

    private void Awake()
    {
        if (circleRenderer == null)
            circleRenderer = GetComponentInChildren<SpriteRenderer>(true);

        if (bonusText == null)
            bonusText = GetComponentInChildren<TMP_Text>(true);

        Hide();
    }

    public void Show(Vector3 worldPosition, float radius, int extraCount)
    {
        worldPosition.z += zOffset;
        transform.position = worldPosition;

        float size = Mathf.Max(0.01f, scaleByDiameter ? radius * 2f : radius);
        transform.localScale = new Vector3(size, size, 1f);

        if (circleRenderer != null)
        {
            circleRenderer.enabled = true;
            circleRenderer.color = extraCount > 0 ? activeColor : emptyColor;
        }

        if (bonusText != null)
        {
            bonusText.gameObject.SetActive(true);
            bonusText.text = extraCount > 0 ? bonusPrefix + extraCount + bonusSuffix : noBonusText;
        }
    }


    public void ShowGlobal(Vector3 worldPosition, int extraCount)
    {
        worldPosition.z += zOffset;
        transform.position = worldPosition;
        transform.localScale = Vector3.one;

        // Режим "вся сцена": круг не показываем, чтобы игрок не думал, что есть радиус.
        if (circleRenderer != null)
            circleRenderer.enabled = false;

        if (bonusText != null)
        {
            bonusText.gameObject.SetActive(true);
            bonusText.text = extraCount > 0
                ? bonusPrefix + extraCount + " вся сцена"
                : noBonusText;
        }
    }

    public void Hide()
    {
        if (circleRenderer != null)
            circleRenderer.enabled = false;

        if (bonusText != null)
            bonusText.gameObject.SetActive(false);

    }
}
