using UnityEngine;

public class Student : Human
{
    [SerializeField] private ListReactions listR;
    [SerializeField] private float speed = 3.5f;

    protected override void Awake()
    {
        SetHumanType("student");
        Name = "student";
        base.Awake();
    }

    protected override string[] GetReactions()
    {
        return listR != null ? listR.NeedStudent : new string[] { "student2" };
    }

    private void Update()
    {
        MoveHuman(speed);
    }
}
