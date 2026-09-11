using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reception : Room
{
    private void Start()
    {
        capacity += Level - 1;
    }
    private void OnEnable()
    {
        var people = GetComponentsInChildren<Person>();

        foreach(var person in people)
        {
            GameManager.Instance.ActiveWorkers.Remove(person);
            GameManager.Instance.Reserve.Remove(person);
            Destroy(person.gameObject);
        }
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
