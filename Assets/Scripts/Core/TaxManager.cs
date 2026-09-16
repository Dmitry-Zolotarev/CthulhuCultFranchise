using UnityEngine;

public class TaxManager : MonoBehaviour
{
    public static TaxManager Instance;
    public int TaxAmount = 0;
    public int TaxRate = 15;
    private void Awake()
    {
        Instance = this;
    }
    public void AddTax(int revenue)
    {
        TaxAmount += revenue * TaxRate / 100;
    }
    public void PayTax()
    {
        int taxToPay = Mathf.Min(GameManager.Instance.Money, TaxAmount);
        GameManager.Instance.TrySpendMoney(taxToPay);
        TaxAmount -= taxToPay;
    }
}
