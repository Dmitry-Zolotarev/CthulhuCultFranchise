using System.Collections;
using UnityEngine;

public class Altar : Room
{
    [SerializeField] private float eatingTime = 6f;

    public override void AssignPerson(Person person)
    {
        base.AssignPerson(person);
        StartCoroutine(SacrificeCoroutine(person));
    }
    private IEnumerator SacrificeCoroutine(Person person)
    {
        if(GameManager.Instance.GetTimeSpeed() > 0)
        {
            yield return new WaitForSeconds(eatingTime / GameManager.Instance.GetTimeSpeed());
            person.Eat(GameManager.Instance.hungerReduction);
        }     
    }
}
