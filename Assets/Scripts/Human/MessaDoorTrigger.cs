using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MessaDoorTrigger : MonoBehaviour
{
    public static MessaDoorTrigger Current { get; private set; }

    [SerializeField] private SpawnHuman spawnHuman;
    [SerializeField] private CounterHuman counterHuman;

    private readonly HashSet<int> acceptedHumanIds = new HashSet<int>();

    private void Awake()
    {
        Current = this;
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnDestroy()
    {
        if (Current == this)
            Current = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Human human = other.GetComponent<Human>();
        if (human == null)
            human = other.GetComponentInParent<Human>();

        if (human == null)
            return;

        if (human.IsGoingToMessa)
            AcceptHuman(human);
    }

    public void AcceptHuman(Human human)
    {
        if (human == null)
            return;

        int id = human.GetInstanceID();
        if (acceptedHumanIds.Contains(id))
            return;

        acceptedHumanIds.Add(id);

        string type = human.HumanType;

        if (GameSessionBridge.Instance != null)
            GameSessionBridge.Instance.AddVisitorToMessa(type);

        if (counterHuman != null)
            counterHuman.AddHuman(1, type);

        if (spawnHuman != null)
            spawnHuman.DeleteHuman(human.gameObject);
        else
            Destroy(human.gameObject);
    }
}
