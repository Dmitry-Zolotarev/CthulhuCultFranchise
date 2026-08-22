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
    void Update()
    {
        if (GameManager.Instance.phase != GamePhase.Office) return;

        if (Time.time >= lastPayTime + payCooldown / GameManager.Instance.GetTimeSpeed()) 
        {
            GameManager.Instance.Money += MoneyPerEmployee * GetCurrentPersonCount();
            lastPayTime = Time.time;
        }
    }
}
