using UnityEngine;

public class Laundry : Room
{
    [SerializeField] private float maxLoyaltyReduction = 0.8f;
    public override void AssignPerson(Person person)
    {
        base.AssignPerson(person);
        person.TryLaunder(maxLoyaltyReduction);
    }
}
