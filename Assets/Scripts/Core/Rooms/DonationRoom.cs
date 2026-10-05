using UnityEngine;

public class DonationRoom : Room
{
    [SerializeField] private int MoneyPerEmployee = 50;
    [SerializeField] private float payCooldown = 6;
    private float lastPayTime;

    private void Start()
    {
        lastPayTime = Time.time;
    }

    private new void Update()
    {
        if (GameManager.Instance.Phase != GamePhase.Office) return;

        if (Time.time >= lastPayTime + payCooldown / GameManager.Instance.GetTimeSpeed()) 
        {
            GameManager.Instance.AddMoney(MoneyPerEmployee * Level * GetCurrentPersonCount());
            lastPayTime = Time.time;
        }
        base.Update();
    }
}
