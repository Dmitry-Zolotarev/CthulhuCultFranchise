using UnityEngine;

// Legacy trigger. Лучше использовать MessaDoorTrigger и StreetExitTrigger.
public class DeleteHumanTrigger : MonoBehaviour
{
    [SerializeField] private SpawnHuman spawnHuman;
    [SerializeField] private CounterHuman counterHuman;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Human human = other.GetComponent<Human>();
        if (human == null)
            human = other.GetComponentInParent<Human>();

        if (human != null)
        {
            if (human.reactionState == 2)
            {
                if (GameSessionBridge.Instance != null)
                    GameSessionBridge.Instance.AddVisitorToMessa(human.HumanType);

                if (counterHuman != null)
                    counterHuman.AddHuman(1, human.HumanType);
            }

            if (spawnHuman != null)
                spawnHuman.DeleteHuman(human.gameObject);
            else
                Destroy(human.gameObject);
        }
    }
}
