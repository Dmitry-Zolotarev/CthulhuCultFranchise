using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StreetExitTrigger : MonoBehaviour
{
    [SerializeField] private SpawnHuman spawnHuman;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Human human = other.GetComponent<Human>();
        if (human == null)
            human = other.GetComponentInParent<Human>();

        if (human != null)
        {
            if (spawnHuman != null)
                spawnHuman.DeleteHuman(human.gameObject);
            else
                Destroy(human.gameObject);
            return;
        }

        PoliceController police = other.GetComponent<PoliceController>();
        if (police == null)
            police = other.GetComponentInParent<PoliceController>();

        if (police != null)
        {
            if (spawnHuman != null)
                spawnHuman.DeleteHuman(police.gameObject);
            else
                Destroy(police.gameObject);
        }
    }
}
