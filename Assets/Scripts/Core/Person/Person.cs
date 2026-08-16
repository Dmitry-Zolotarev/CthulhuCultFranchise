using UnityEngine;

public class Person : MonoBehaviour
{
    [Header("Person")]
    public PersonType type;

    [Range(1, 5)]
    public int loyalty = 1;

    [Range(0, 5)]
    public int Suspicion = 0;

    public RoomType currentRoom = RoomType.Reception;

    public int contacts = 1;

    [Header("State")]
    public bool signedContract;
    public bool escapeWarning;
    public bool isBeingSacrificed;

    public bool IsStudent =>
        type == PersonType.Student;

    public bool IsOfficeWorker =>
        type == PersonType.OfficeWorker;

    public float Efficiency
    {
        get
        {
            float value = 1f;

            if (IsStudent &&
                currentRoom == RoomType.Agitation)
            {
                value += 0.5f;
            }

            if (IsOfficeWorker &&
                currentRoom == RoomType.Donations)
            {
                value += 0.5f;
            }

            value += (loyalty - 1) * 0.05f;

            return value;
        }
    }

    public void SignContract()
    {
        signedContract = true;
        currentRoom = RoomType.Reception;
    }

    public void AddSuspicion(int amount)
    {
        Suspicion = Mathf.Clamp(
            Suspicion + amount,
            0,
            5
        );

        if (Suspicion >= 5)
        {
            TriggerEscapeWarning();
        }
    }

    public void RemoveSuspicion(int amount)
    {
        Suspicion = Mathf.Max(
            0,
            Suspicion - amount
        );

        if (Suspicion < 5)
        {
            escapeWarning = false;
        }
    }

    public void TriggerEscapeWarning()
    {
        if (escapeWarning)
            return;

        escapeWarning = true;

        Debug.Log(
            $"{name}: подозрение достигло 5."
        );

        // UI кризиса можно подключить здесь.
    }

    public void CancelEscape()
    {
        escapeWarning = false;

        Suspicion = Mathf.Min(
            Suspicion,
            4
        );
    }

    public void Escape()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.reserve.Remove(this);
        GameManager.Instance.activeWorkers.Remove(this);

        GameManager.Instance.AddAnxiety(1);

        Destroy(gameObject);
    }

    public void Sacrifice()
    {
        if (isBeingSacrificed)
            return;

        isBeingSacrificed = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddGrace(10);
            GameManager.Instance.ReduceHunger(30);

            GameManager.Instance.reserve.Remove(this);
            GameManager.Instance.activeWorkers.Remove(this);
        }

        Destroy(gameObject);
    }

    public void Eat()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddAnxiety(1);
            GameManager.Instance.ReduceHunger(50);

            GameManager.Instance.reserve.Remove(this);
            GameManager.Instance.activeWorkers.Remove(this);
        }

        Destroy(gameObject);
    }
}