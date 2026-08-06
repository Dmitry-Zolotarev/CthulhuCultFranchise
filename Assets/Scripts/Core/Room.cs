using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomType roomType;
    public int capacity = 2;
    public bool unlocked = true;
    public bool repaired = true;

    private Person worker;
    private float timer;

    public bool HasWorker => worker != null;

    public void Assign(Person person)
    {
        if (!unlocked || !repaired || person == null)
            return;

        worker = person;
        person.currentRoom = roomType;
        timer = GetCycleTime();
    }

    public void RemoveWorker()
    {
        if (worker == null) return;

        worker = null;
        timer = 0f;
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.phase != GamePhase.Office ||
            worker == null)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (roomType == RoomType.Altar)
                CompleteAltar();
            else
            {
                CompleteCycle();
                timer = GetCycleTime();
            }
        }
    }

    private float GetCycleTime()
    {
        switch (roomType)
        {
            case RoomType.Donations: return 6f;
            case RoomType.Propaganda: return 8f;
            case RoomType.Laundry: return 7f;
            case RoomType.Altar: return 4f;
            default: return 999f;
        }
    }

    private void CompleteCycle()
    {
        if (worker == null) return;

        switch (roomType)
        {
            case RoomType.Donations:
                DoDonation();
                break;
            case RoomType.Propaganda:
                DoPropaganda();
                break;
            case RoomType.Laundry:
                DoLaundry();
                break;
        }
    }

    private void DoDonation()
    {
        int income = Mathf.RoundToInt(100 * worker.Efficiency);
        GameManager.Instance.AddMoney(income);
        worker.AddSuspicion(1);
        Debug.Log($"{worker.name}: пожертвования +{income} денег.");
    }

    private void DoPropaganda()
    {
        if (worker.contacts <= 0) return;

        worker.contacts--;

        if (worker.IsStudent)
            GameManager.Instance.universityProgress++;
        else
            GameManager.Instance.businessProgress++;

        GameManager.Instance.RegisterContactConverted();
        Debug.Log($"{worker.name}: контакт обращён.");
    }

    private void DoLaundry()
    {
        worker.RemoveSuspicion(1);
        Debug.Log($"{worker.name}: подозрение -1.");

        if (worker.escapeWarning)
        {
            worker.CancelEscape();
            Time.timeScale = 1f;
        }
    }

    private void CompleteAltar()
    {
        if (worker == null) return;

        worker.Sacrifice();
        worker = null;
    }

    public void StartAltar(Person person)
    {
        if (roomType != RoomType.Altar) return;
        Assign(person);
    }
}
