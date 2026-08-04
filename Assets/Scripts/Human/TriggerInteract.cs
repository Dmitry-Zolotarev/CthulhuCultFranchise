using UnityEngine;

public class TriggerInteract : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Human"))
        {
            Human human = collision.GetComponent<Human>();
            if (human == null)
                human = collision.GetComponentInParent<Human>();

            if (human != null && human.CanReceiveFlyer())
            {
                human.interact = true;

                if (human.emotion != null && human.emotion.Length > 0 && human.emotion[0] != null)
                    human.emotion[0].SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Human"))
        {
            Human human = collision.GetComponent<Human>();
            if (human == null)
                human = collision.GetComponentInParent<Human>();

            if (human != null)
            {
                human.interact = false;

                if (human.emotion != null && human.emotion.Length > 0 && human.emotion[0] != null)
                    human.emotion[0].SetActive(false);
            }
        }
    }
}
