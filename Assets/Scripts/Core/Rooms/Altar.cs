using System.Collections;
using UnityEngine;

public class Altar : Room
{
    [SerializeField] private float eatingTime = 6f;

    public override void AssignPerson(Person person)
    {
        if (GameManager.Instance.Hunger < OfficeManager.Instance.hungerReduction / 1.5f) return;
        base.AssignPerson(person);
        StartCoroutine(SacrificeCoroutine(person));
    }
    private IEnumerator SacrificeCoroutine(Person person)
    {
        if(OfficeManager.Instance.GetTimeSpeed() > 0)
        {
            yield return new WaitForSeconds(eatingTime / OfficeManager.Instance.GetTimeSpeed());
            person.Eat(OfficeManager.Instance.hungerReduction);
        }     
    }
}
