using UnityEngine;

public class TaxManager : MonoBehaviour
{
    public static TaxManager Instance;
    public int TodayTax = 0;
    public int TaxRate = 15;
    private void Awake()
    {
        Instance = this;
    }
    public void AddTax(int revenue)
    {
        TodayTax += revenue * TaxRate / 100;
    }
    public void PayTax()
    {
        int taxToPay = Mathf.Min(GameManager.Instance.Money, TodayTax);
        GameManager.Instance.TrySpendMoney(taxToPay);
        TodayTax -= taxToPay;
    }
}
