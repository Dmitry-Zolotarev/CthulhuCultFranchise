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
            person.Quit();
            return;
        }
        base.AssignPerson(person);
    }
}
