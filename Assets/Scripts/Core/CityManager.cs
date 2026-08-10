using UnityEngine;

public enum District
{
    University,
    BusinessCenter
}

public class CityManager : MonoBehaviour
{
    public static CityManager Instance { get; private set; }

    // =========================================================
    // STATE
    // =========================================================

    [Header("Map Actions")]
    public bool advertisingUsed;
    public bool secretMeetingUsed;
    public bool coverUsed;

    // =========================================================
    // COSTS
    // =========================================================

    [Header("Advertising")]
    [SerializeField] private int advertisingInfluenceCost = 1;
    [SerializeField] private int advertisingMoneyCost = 100;
    [SerializeField] private int advertisingVisitors = 2;

    [Header("Secret Meeting")]
    [SerializeField] private int secretMeetingInfluenceCost = 1;
    [SerializeField] private int secretMeetingVisitors = 1;

    [Header("Cover")]
    [SerializeField] private int coverInfluenceCost = 1;
    [SerializeField] private int coverMoneyCost = 150;

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // =========================================================
    // NEW DAY
    // =========================================================

    public void ResetDayActions()
    {
        advertisingUsed = false;
        secretMeetingUsed = false;
        coverUsed = false;
    }

    // =========================================================
    // ADVERTISING
    // =========================================================

    public void Advertising(District district)
    {
        if (GameManager.Instance == null)
            return;

        if (advertisingUsed)
        {
            Debug.Log(
                "Реклама уже использована сегодня."
            );

            return;
        }

        if (!CanPay(
                advertisingInfluenceCost,
                advertisingMoneyCost))
        {
            Debug.Log(
                "Недостаточно ресурсов для рекламы."
            );

            return;
        }

        Pay(
            advertisingInfluenceCost,
            advertisingMoneyCost
        );

        advertisingUsed = true;

        if (OfficeManager.Instance != null)
        {
            OfficeManager.Instance.AddExtraVisitors(
                advertisingVisitors
            );
        }

        Debug.Log(
            $"Реклама в районе {district}. " +
            $"Следующая смена: +" +
            $"{advertisingVisitors} посетителя."
        );
    }

    // =========================================================
    // SECRET MEETING
    // =========================================================

    public void SecretMeeting(District district)
    {
        if (GameManager.Instance == null)
            return;

        // По твоей текущей логике —
        // доступно начиная со 2-го дня.
        if (GameManager.Instance.day < 2)
        {
            Debug.Log(
                "Тайная встреча доступна со 2-го дня."
            );

            return;
        }

        if (secretMeetingUsed)
        {
            Debug.Log(
                "Тайная встреча уже использована сегодня."
            );

            return;
        }

        if (!CanPay(
                secretMeetingInfluenceCost,
                0))
        {
            Debug.Log(
                "Недостаточно влияния для тайной встречи."
            );

            return;
        }

        Pay(
            secretMeetingInfluenceCost,
            0
        );

        secretMeetingUsed = true;

        if (district == District.University)
        {
            GameManager.Instance.universityProgress++;
        }
        else
        {
            GameManager.Instance.businessProgress++;
        }

        if (OfficeManager.Instance != null)
        {
            OfficeManager.Instance.AddExtraVisitors(
                secretMeetingVisitors
            );
        }

        Debug.Log(
            $"Тайная встреча в {district}. " +
            $"Прогресс района +1. " +
            $"Следующая смена: +" +
            $"{secretMeetingVisitors} посетитель."
        );
    }

    // =========================================================
    // COVER
    // =========================================================

    public void Cover()
    {
        if (GameManager.Instance == null)
            return;

        // Доступно только с 3-го дня.
        if (GameManager.Instance.day < 3)
        {
            Debug.Log(
                "Прикрытие доступно только на 3-й день."
            );

            return;
        }

        if (coverUsed)
        {
            Debug.Log(
                "Прикрытие уже использовано сегодня."
            );

            return;
        }

        if (GameManager.Instance.Anxiety <= 0)
        {
            Debug.Log(
                "Тревога уже равна 0."
            );

            return;
        }

        if (!CanPay(
                coverInfluenceCost,
                coverMoneyCost))
        {
            Debug.Log(
                "Недостаточно ресурсов для прикрытия."
            );

            return;
        }

        Pay(
            coverInfluenceCost,
            coverMoneyCost
        );

        coverUsed = true;

        GameManager.Instance.AddAnxiety(-1);

        Debug.Log(
            "Прикрытие: Тревога -1."
        );
    }

    // =========================================================
    // PAYMENT
    // =========================================================

    private bool CanPay(
        int influenceCost,
        int moneyCost)
    {
        if (GameManager.Instance.Influence <
            influenceCost)
        {
            return false;
        }

        if (GameManager.Instance.Money <
            moneyCost)
        {
            return false;
        }

        return true;
    }

    private void Pay(
        int influenceCost,
        int moneyCost)
    {
        if (influenceCost > 0)
        {
            GameManager.Instance.SpendInfluence(
                influenceCost
            );
        }

        if (moneyCost > 0)
        {
            GameManager.Instance.SpendMoney(
                moneyCost
            );
        }
    }

    // =========================================================
    // AVAILABILITY
    // =========================================================

    public bool CanUseAdvertising()
    {
        if (GameManager.Instance == null)
            return false;

        if (advertisingUsed)
            return false;

        return CanPay(
            advertisingInfluenceCost,
            advertisingMoneyCost
        );
    }

    public bool CanUseSecretMeeting()
    {
        if (GameManager.Instance == null)
            return false;

        if (GameManager.Instance.day < 2)
            return false;

        if (secretMeetingUsed)
            return false;

        return CanPay(
            secretMeetingInfluenceCost,
            0
        );
    }

    public bool CanUseCover()
    {
        if (GameManager.Instance == null)
            return false;

        if (GameManager.Instance.day < 3)
            return false;

        if (coverUsed)
            return false;

        if (GameManager.Instance.Anxiety <= 0)
            return false;

        return CanPay(
            coverInfluenceCost,
            coverMoneyCost
        );
    }

    // =========================================================
    // DISTRICT PROGRESS
    // =========================================================

    public int GetDistrictProgress(
        District district)
    {
        if (GameManager.Instance == null)
            return 0;

        if (district == District.University)
        {
            return GameManager.Instance
                .universityProgress;
        }

        return GameManager.Instance
            .businessProgress;
    }

    // =========================================================
    // RESET
    // =========================================================

    public void ResetAllActions()
    {
        advertisingUsed = false;
        secretMeetingUsed = false;
        coverUsed = false;
    }
}