using UnityEngine;

public class Retiree : Human
{
    [SerializeField] private ListReactions listR;
    [SerializeField] private float speed = 2.8f;

    protected override void Awake()
    {
        SetHumanType("retiree");
        Name = "retiree";
        base.Awake();
    }

    protected override string[] GetReactions()
    {
        return listR != null ? listR.NeedRetiree : new string[] { "retiree2" };
    }

    private void Update()
    {
        MoveHuman(speed);
    }
}
