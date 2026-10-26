using UnityEngine;
using UnityEngine.UI;
public class TaxManager : MonoBehaviour
{
    public static TaxManager Instance;   
    public int TaxAmount = 0;
    public int TaxRate = 15;
    [SerializeField] private Button payTaxButton;
    private void Awake()
    {
        Instance = this;
    }
    public void AddTax(int revenue)
    {
        TaxAmount += revenue * TaxRate / 100;
    }
    public void Update()
    {
        payTaxButton.interactable = TaxAmount > 0 && GameManager.Instance.Money >= TaxAmount;
    }
    public void PayTax()
    {
        int taxToPay = Mathf.Min(GameManager.Instance.Money, TaxAmount);
        if(GameManager.Instance.TrySpendMoney(taxToPay))
        {
            TaxAmount -= taxToPay;
        }
    }
}
