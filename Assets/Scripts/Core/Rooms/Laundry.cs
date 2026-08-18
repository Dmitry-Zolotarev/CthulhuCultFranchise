using System.Collections;
using UnityEngine;

public class Laundry : Room
{
    [SerializeField] private float launderingTime = 2f;
    [SerializeField] private float maxLoyaltyReduction = 0.8f;
    public override void AssignPerson(Person person)
    {
        if (person.loyalty >= person.maxLoyalty * maxLoyaltyReduction) return;
        base.AssignPerson(person);
        StartCoroutine(LaundryCoroutine(person));
    }
    private IEnumerator LaundryCoroutine(Person person)
    {
        if(OfficeManager.Instance.GetTimeSpeed() > 0)
        {
            yield return new WaitForSeconds(launderingTime / OfficeManager.Instance.GetTimeSpeed());
            person.maxLoyalty *= maxLoyaltyReduction;
            person.loyalty = person.maxLoyalty;
        }     
    }
}
