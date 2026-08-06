using UnityEngine;

public class DayManager : MonoBehaviour
{
    public OfficeManager officeManager;

    public void OpenPreparation()
    {
        GameManager.Instance.phase = GamePhase.Preparation;
        GameManager.Instance.activeWorkers.Clear();

        Debug.Log("Выберите до 4 работников.");
    }

    public bool SelectWorker(Person person)
    {
        if (GameManager.Instance.activeWorkers.Count >= 4)
            return false;

        if (!GameManager.Instance.reserve.Contains(person))
            return false;

        if (GameManager.Instance.activeWorkers.Contains(person))
            return false;

        GameManager.Instance.activeWorkers.Add(person);
        return true;
    }

    public void StartOffice()
    {
        if (GameManager.Instance.activeWorkers.Count > 4)
            return;

        officeManager.StartShift();
    }

    public void SignAndAcceptResponsibility()
    {
        if (GameManager.Instance.phase != GamePhase.Report)
            return;

        if (GameManager.Instance.day == 3)
        {
            ShowFinalResult();
            return;
        }

        GameManager.Instance.NextDay();
        Debug.Log($"Начинается день {GameManager.Instance.day}.");
    }

    private void ShowFinalResult()
    {
        bool success = GameManager.Instance.IsKPIComplete();

        Debug.Log(
            success
                ? "KPI выполнен. Игрок выбирает бонус."
                : "KPI провален. Игрок выбирает наказание."
        );

        GameManager.Instance.phase = GamePhase.Final;
    }
}
