using UnityEngine;

public class Laundry : Room
{
    public float maxLoyaltyReduction = 0.8f;
    public override void AssignPerson(Person person)
    {
        base.AssignPerson(person);
        if (person.loyalty < person.maxLoyalty * maxLoyaltyReduction) person.maxLoyalty *= maxLoyaltyReduction;
    }
}
