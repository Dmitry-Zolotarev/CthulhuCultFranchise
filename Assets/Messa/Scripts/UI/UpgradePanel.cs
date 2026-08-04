using UnityEngine;
using UnityEngine.UI;
using TMPro;
[RequireComponent(typeof(Image))]
public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI Header;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI priceLabel;

    public void BindUpgrade(int i)
    {      
        if (i >= Messa.Instance.UpgradeList.Length) return;
        var upgrade = Messa.Instance.UpgradeList[i];

        Header?.SetText(upgrade.Header);
        description?.SetText(upgrade.Description);
        priceLabel?.SetText($"Купить за ${upgrade.Price}");
        icon.sprite = upgrade.Icon;

        if(buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => Messa.Instance.BuyUpgrade(i));
        }      
    }
}
