using UnityEngine;

public enum District
{
    University,
    BusinessCenter
}

public class CityManager : MonoBehaviour
{
    public bool advertisingUsed;
    public bool secretMeetingUsed;
    public bool coverUsed;

    public void Advertising(District district)
    {
        if (!GameManager.Instance.SpendInfluence(1))
            return;

        if (!GameManager.Instance.SpendMoney(100))
            return;

        advertisingUsed = true;
        OfficeManager.Instance.AddExtraVisitors(2);

        Debug.Log(
            $"Реклама в районе {district}. Следующая смена: +2 посетителя."
        );
    }

    public void SecretMeeting(District district)
    {
        if (GameManager.Instance.day < 2)
            return;

        if (!GameManager.Instance.SpendInfluence(1))
            return;

        secretMeetingUsed = true;

        if (district == District.University)
            GameManager.Instance.universityProgress++;
        else
            GameManager.Instance.businessProgress++;

        OfficeManager.Instance.AddExtraVisitors(1);

        Debug.Log(
            $"Тайная встреча в {district}. +1 посетитель следующей смены."
        );
    }

    public void Cover()
    {
        if (GameManager.Instance.day < 3)
            return;

        if (GameManager.Instance.anxiety <= 0)
            return;

        if (!GameManager.Instance.SpendInfluence(1))
            return;

        if (!GameManager.Instance.SpendMoney(150))
            return;

        coverUsed = true;
        GameManager.Instance.AddAnxiety(-1);

        Debug.Log("Прикрытие: Тревога -1.");
    }
}
