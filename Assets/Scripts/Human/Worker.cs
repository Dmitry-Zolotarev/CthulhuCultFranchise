using UnityEngine;

public class Worker : Human
{
    [SerializeField] private ListReactions listR;
    [SerializeField] private float speed = 3.5f;

    protected override void Awake()
    {
        SetHumanType("worker");
        Name = "worker";
        base.Awake();
    }

    protected override string[] GetReactions()
    {
        return listR != null ? listR.NeedWorker : new string[] { "worker2" };
    }

    private void Update()
    {
        MoveHuman(speed);
    }
}
