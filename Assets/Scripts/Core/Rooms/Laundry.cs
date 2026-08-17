using System.Collections;
using UnityEngine;

public class Laundry : Room
{
    [SerializeField] private float launderingTime = 2f;

    public override void AssignPerson(Person person)
    {
        if (person.loyalty > person.maxLoyalty / 3f) return;
        base.AssignPerson(person);
        StartCoroutine(LaundryCoroutine(person));
    }
    private IEnumerator LaundryCoroutine(Person person)
    {
        if(OfficeManager.Instance.GetTimeSpeed() > 0)
        {
            yield return new WaitForSeconds(launderingTime / OfficeManager.Instance.GetTimeSpeed());
            person.maxLoyalty /= 2;
            person.loyalty = person.maxLoyalty;
        }     
    }
}
