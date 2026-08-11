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
        roomType = RoomType.Donations;
        lastPayTime = Time.time;
    }
    void Update()
    {
        if (Time.time >= lastPayTime + payCooldown) 
        {
            GameManager.Instance.Money += MoneyPerEmployee * GetCurrentPersonCount();
            lastPayTime = Time.time;
        }
    }
}
