using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reception : Room
{
    private void Start()
    {
        capacity += Level - 1;
    }
    public override void AssignPerson(Person person)
    {
        if(person.IsCultist && !(person.Room is Reception))
        {        
            if (person.Loyalty / person.MaxLoyalty > 0.3f)
            {
                GameManager.Instance.TodayReleased++;
            }
            else
            {
                GameManager.Instance.TodayEscaped++;
            }
            GameManager.Instance.Reserve.Remove(person);
            GameManager.Instance.ActiveWorkers.Remove(person);
            Destroy(person.gameObject);
            return;
        }
        base.AssignPerson(person);
    }
}
