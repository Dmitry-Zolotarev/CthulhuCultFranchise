using UnityEngine;

public class Esoteric : Human
{
    [SerializeField] private ListReactions listR;
    [SerializeField] private float speed = 3.2f;

    protected override void Awake()
    {
        SetHumanType("esoteric");
        Name = "esoteric";
        base.Awake();
    }

    protected override string[] GetReactions()
    {
        return listR != null ? listR.NeedEsoteric : new string[] { "esoteric2" };
    }

    private void Update()
    {
        MoveHuman(speed);
    }
}
