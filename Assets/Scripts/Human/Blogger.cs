using UnityEngine;

public class Blogger : Human
{
    [SerializeField] private ListReactions listR;
    [SerializeField] private float speed = 3.8f;

    protected override void Awake()
    {
        SetHumanType("blogger");
        Name = "blogger";
        base.Awake();
    }

    protected override string[] GetReactions()
    {
        return listR != null ? listR.NeedBlogger : new string[] { "blogger2" };
    }

    private void Update()
    {
        MoveHuman(speed);
    }
}
