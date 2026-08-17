using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonationRoom : Room
{
    // Start is called before the first frame update
    
    [SerializeField] private int MoneyPerEmployee = 50;
    [SerializeField] private float payCooldown = 6;
    private float lastPayTime;

    private void Start()
    {
        lastPayTime = Time.time;
    }
    void Update()
    {
        if (!OfficeManager.Instance.ShiftRunning) return;

        if (Time.time >= lastPayTime + payCooldown / OfficeManager.Instance.GetTimeSpeed()) 
        {
            GameManager.Instance.Money += MoneyPerEmployee * GetCurrentPersonCount();
            lastPayTime = Time.time;
        }
    }
}
