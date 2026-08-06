using UnityEngine;

public class Person : MonoBehaviour
{
    public PersonType type;

    [Range(1, 5)]
    public int loyalty = 1;

    [Range(0, 5)]
    public int Suspicion = 0;

    public RoomType currentRoom = RoomType.Reception;
    public int contacts = 1;

    public bool signedContract;
    public bool escapeWarning;
    public bool isBeingSacrificed;

    public bool IsStudent => type == PersonType.Student;
    public bool IsOfficeWorker => type == PersonType.OfficeWorker;

    public float Efficiency
    {
        get
        {
            float value = 1f;

            if (IsStudent && currentRoom == RoomType.Propaganda)
                value += 0.5f;

            if (IsOfficeWorker && currentRoom == RoomType.Donations)
                value += 0.5f;

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
        Suspicion = Mathf.Clamp(Suspicion + amount, 0, 5);

        if (Suspicion >= 5)
            TriggerEscapeWarning();
    }

    public void RemoveSuspicion(int amount)
    {
        Suspicion = Mathf.Max(0, Suspicion - amount);

        if (Suspicion < 5)
            escapeWarning = false;
    }

    public void TriggerEscapeWarning()
    {
        if (escapeWarning) return;

        escapeWarning = true;
        GameManager.Instance.phase = GamePhase.Office;

        Debug.Log($"{name}: ПРЕДУПРЕЖДЕНИЕ ПОБЕГА. Подозрение = 5.");
        Time.timeScale = 0f;
    }

    public void CancelEscape()
    {
        escapeWarning = false;
        Suspicion = Mathf.Min(Suspicion, 4);
    }

    public void Escape()
    {
        GameManager.Instance.reserve.Remove(this);
        GameManager.Instance.activeWorkers.Remove(this);
        GameManager.Instance.AddAnxiety(1);
        Destroy(gameObject);
    }

    public void Sacrifice()
    {
        if (isBeingSacrificed) return;

        isBeingSacrificed = true;

        GameManager.Instance.AddGrace(10);
        GameManager.Instance.ReduceHunger(30);

        GameManager.Instance.reserve.Remove(this);
        GameManager.Instance.activeWorkers.Remove(this);

        Destroy(gameObject);
    }

    public void Eat()
    {
        GameManager.Instance.AddAnxiety(1);
        GameManager.Instance.ReduceHunger(50);

        GameManager.Instance.reserve.Remove(this);
        GameManager.Instance.activeWorkers.Remove(this);

        Destroy(gameObject);
    }
}
