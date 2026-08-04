using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandingFlyers : MonoBehaviour
{
    public static HandingFlyers singltoneFlyer { get; private set; }

    [Header("UI")]
    [SerializeField] private Text suspicionText;
    [SerializeField] private TMP_Text suspicionTMPText;
    [SerializeField] private Image suspicionFillImage;
    [SerializeField] private string suspicionPrefix = "Предупреждения полиции: ";

    [Header("Throw")]
    [SerializeField] private CultistFlyerThrower cultistThrower;

    [Header("Bridge")]
    [SerializeField] private bool useBridgeUpgradeToggles = true;

    [Header("Combo")]
    [SerializeField] private ComboSystem comboSystem;

    [Header("Megaphone")]
    [SerializeField] private bool isMegafon = false;
    [Tooltip("Финальная логика: мегафон цепляет ВСЕХ подходящих прохожих на сцене, а не только в радиусе.")]
    [SerializeField] private bool megafonAffectsWholeScene = true;
    [Tooltip("Старая логика радиуса оставлена для отката. Работает только если Megafon Affects Whole Scene выключен.")]
    [SerializeField] private float radiusMegafon = 2f;
    [SerializeField] private LayerMask layer;
    [Tooltip("Работает только в режиме радиуса. В режиме Whole Scene игнорируется, потому что цепляются все подходящие на сцене.")]
    [SerializeField] private int maxMegafonExtraHumans = 1;
    [SerializeField] private bool megafonOnlyIfMainTargetCorrect = true;
    [SerializeField] private FlyerVisualDatabase flyerVisualDatabase;

    [Header("Megaphone Preview")]
    [SerializeField] private MegafonAreaPreview megafonAreaPreview;
    [SerializeField] private bool showMegafonPreview = true;
    [SerializeField] private bool showPreviewEvenWhenNoExtraTargets = false;
    [SerializeField] private float previewYOffset = 0f;

    [Header("Police Suspicion")]
    public bool isPolice = true;
    [SerializeField] private bool suspicionOnlyInsidePoliceZone = true;
    [Tooltip("Лимит предупреждений. Любая применённая листовка в зоне полиции даёт +1 предупреждение, даже если листовка правильная.")]
    [SerializeField] private int wrongFlyersToCatch = 2;
    [Tooltip("Если есть бонус Клуб саморазвития, лимит предупреждений увеличивается на это число. 2 + 1 = 3 предупреждения до поимки.")]
    [SerializeField] private int selfImprovementExtraWrongFlyers = 1;

    [Header("Devil Advocate")]
    [SerializeField] private bool hasDevilAdvocate = false;
    [Tooltip("Адвокат дьявола 1 раз в день блокирует поимку и уменьшает подозрение на это число. Было 2 -> стало 1.")]
    [SerializeField] private int devilAdvocateReduceWrongFlyers = 1;

    [Header("Police Catch")]
    [SerializeField] private DayController dayController;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private string policeCatchReason = "Полиция накрыла точку";
    [SerializeField] private string devilAdvocateMessage = "Адвокат дьявола отменил поимку";
    [Tooltip("Необязательно. Если назначить SpawnHuman, при поимке полиция остановит новый спавн сразу, пока бежит к точке.")]
    [SerializeField] private SpawnHuman spawnHuman;

    private float currentSuspicion = 0f;
    private bool devilAdvocateUsedToday = false;
    private bool streetLocked = true;

    private bool IsMegafonActive =>
        useBridgeUpgradeToggles && GameSessionBridge.Instance != null
            ? GameSessionBridge.Instance.HasMegafon
            : isMegafon;

    private bool HasDevilAdvocateActive =>
        useBridgeUpgradeToggles && GameSessionBridge.Instance != null
            ? GameSessionBridge.Instance.HasDevilAdvocate
            : hasDevilAdvocate;

    private bool HasSelfImprovementClubActive =>
        useBridgeUpgradeToggles && GameSessionBridge.Instance != null
            ? GameSessionBridge.Instance.HasSelfImprovementClub
            : false;

    private void Awake()
    {
        singltoneFlyer = this;
    }

    private void OnEnable()
    {
        DayController.dayEnd += OnDayEnd;
        streetLocked = false;
        UpdateSuspicionUI();
    }

    private void OnDisable()
    {
        DayController.dayEnd -= OnDayEnd;
        HideMegafonPreview();
    }

    private void Update()
    {
        if (streetLocked)
        {
            HideMegafonPreview();
            return;
        }

        UpdateMegafonPreview();

        if (Input.GetMouseButtonDown(0))
            TryStartThrow();
    }

    private void OnDayEnd(bool ended)
    {
        if (ended)
        {
            streetLocked = true;
            HideMegafonPreview();
            return;
        }

        streetLocked = false;
        devilAdvocateUsedToday = false;
        currentSuspicion = 0f;
        UpdateSuspicionUI();
    }

    private void TryStartThrow()
    {
        if (FlyersController.singltoneFlyers == null)
            return;

        string selectedFlyer = FlyersController.singltoneFlyers.CurrentFlyer();
        if (string.IsNullOrEmpty(selectedFlyer))
            return;

        if (cultistThrower != null && cultistThrower.IsThrowing)
            return;

        Human human = GetHumanUnderMouse();
        if (human == null)
            return;

        if (!human.ReserveForFlyerThrow())
            return;

        bool mainTargetCorrect = IsCorrectFlyerForHuman(human, selectedFlyer);

        TryAddPoliceSuspicion(human, selectedFlyer);

        // ВАЖНО: выбранная листовка НЕ сбрасывается после броска.
        // Игрок выбирает 1/2/3/4/5 или кнопкой и кидает её сколько хочет, пока не выберет другую.

        if (cultistThrower != null)
        {
            cultistThrower.ThrowFlyerTo(
                human,
                selectedFlyer,
                () => OnMainFlyerHit(human, selectedFlyer, mainTargetCorrect)
            );
        }
        else
        {
            human.ReceiveFlyer(selectedFlyer, GetBubbleSprite(selectedFlyer));
            OnMainFlyerHit(human, selectedFlyer, mainTargetCorrect);
        }
    }

    private void OnMainFlyerHit(Human mainHuman, string selectedFlyer, bool mainTargetCorrect)
    {
        if (mainTargetCorrect)
        {
            int extraRecruited = ApplyMegafon(mainHuman, selectedFlyer, mainTargetCorrect);
            RegisterComboGain(1 + extraRecruited);
        }
        else
        {
            RegisterComboMiss();
        }
    }

    private int ApplyMegafon(Human mainHuman, string selectedFlyer, bool mainTargetCorrect)
    {
        if (!IsMegafonActive || mainHuman == null)
            return 0;

        if (megafonOnlyIfMainTargetCorrect && !mainTargetCorrect)
            return 0;

        int recruitedCount = 0;
        Sprite bubbleSprite = GetBubbleSprite(selectedFlyer);

        if (megafonAffectsWholeScene)
        {
            // Финальная логика: без радиуса.
            // Если основная листовка правильная, мегафон цепляет ВСЕХ подходящих прохожих,
            // которые сейчас активны на сцене.
            Human[] allHumans = FindObjectsOfType<Human>();

            for (int i = 0; i < allHumans.Length; i++)
            {
                Human otherHuman = allHumans[i];

                if (otherHuman == null || otherHuman == mainHuman)
                    continue;

                if (!otherHuman.CanBeMegafonRecruited())
                    continue;

                if (!IsCorrectFlyerForHuman(otherHuman, selectedFlyer))
                    continue;

                otherHuman.ReceiveFlyerFromMegafon(selectedFlyer, bubbleSprite);
                recruitedCount++;
            }
        }
        else
        {
            if (maxMegafonExtraHumans <= 0)
                return 0;

            Collider2D[] hits = Physics2D.OverlapCircleAll(mainHuman.transform.position, radiusMegafon, layer);

            for (int i = 0; i < hits.Length; i++)
            {
                Human otherHuman = GetHumanFromCollider(hits[i]);

                if (otherHuman == null || otherHuman == mainHuman)
                    continue;

                if (!otherHuman.CanBeMegafonRecruited())
                    continue;

                if (!IsCorrectFlyerForHuman(otherHuman, selectedFlyer))
                    continue;

                otherHuman.ReceiveFlyerFromMegafon(selectedFlyer, bubbleSprite);
                recruitedCount++;

                if (recruitedCount >= maxMegafonExtraHumans)
                    break;
            }
        }

        if (recruitedCount > 0)
            Debug.Log("Мегафон завербовал дополнительных прохожих: " + recruitedCount + ". Комбо +" + recruitedCount);

        return recruitedCount;
    }

    private void RegisterComboGain(int amount)
    {
        ComboSystem targetComboSystem = comboSystem != null ? comboSystem : ComboSystem.Instance;
        if (targetComboSystem != null)
            targetComboSystem.RegisterFlyerResult(true, amount);
    }

    private void RegisterComboMiss()
    {
        ComboSystem targetComboSystem = comboSystem != null ? comboSystem : ComboSystem.Instance;
        if (targetComboSystem != null)
            targetComboSystem.RegisterFlyerResult(false);
    }

    private void UpdateMegafonPreview()
    {
        if (!showMegafonPreview || megafonAreaPreview == null || !IsMegafonActive)
        {
            HideMegafonPreview();
            return;
        }

        if (FlyersController.singltoneFlyers == null)
        {
            HideMegafonPreview();
            return;
        }

        string selectedFlyer = FlyersController.singltoneFlyers.CurrentFlyer();
        if (string.IsNullOrEmpty(selectedFlyer))
        {
            HideMegafonPreview();
            return;
        }

        Human hoveredHuman = GetHumanUnderMouse();
        if (hoveredHuman == null || !hoveredHuman.CanReceiveFlyer())
        {
            HideMegafonPreview();
            return;
        }

        bool targetCorrect = IsCorrectFlyerForHuman(hoveredHuman, selectedFlyer);
        if (megafonOnlyIfMainTargetCorrect && !targetCorrect)
        {
            HideMegafonPreview();
            return;
        }

        int extraTargets = CountMegafonExtraTargets(hoveredHuman, selectedFlyer);
        if (extraTargets <= 0 && !showPreviewEvenWhenNoExtraTargets)
        {
            HideMegafonPreview();
            return;
        }

        Vector3 previewPosition = hoveredHuman.transform.position;
        previewPosition.y += previewYOffset;

        if (megafonAffectsWholeScene)
            megafonAreaPreview.ShowGlobal(previewPosition, extraTargets);
        else
            megafonAreaPreview.Show(previewPosition, radiusMegafon, extraTargets);
    }

    private int CountMegafonExtraTargets(Human mainHuman, string selectedFlyer)
    {
        if (mainHuman == null)
            return 0;

        int count = 0;

        if (megafonAffectsWholeScene)
        {
            Human[] allHumans = FindObjectsOfType<Human>();

            for (int i = 0; i < allHumans.Length; i++)
            {
                Human otherHuman = allHumans[i];

                if (otherHuman == null || otherHuman == mainHuman)
                    continue;

                if (!otherHuman.CanBeMegafonRecruited())
                    continue;

                if (!IsCorrectFlyerForHuman(otherHuman, selectedFlyer))
                    continue;

                count++;
            }

            return count;
        }

        if (maxMegafonExtraHumans <= 0)
            return 0;

        Collider2D[] hits = Physics2D.OverlapCircleAll(mainHuman.transform.position, radiusMegafon, layer);

        for (int i = 0; i < hits.Length; i++)
        {
            Human otherHuman = GetHumanFromCollider(hits[i]);

            if (otherHuman == null || otherHuman == mainHuman)
                continue;

            if (!otherHuman.CanBeMegafonRecruited())
                continue;

            if (!IsCorrectFlyerForHuman(otherHuman, selectedFlyer))
                continue;

            count++;
            if (count >= maxMegafonExtraHumans)
                break;
        }

        return count;
    }

    private void HideMegafonPreview()
    {
        if (megafonAreaPreview != null)
            megafonAreaPreview.Hide();
    }

    private Human GetHumanUnderMouse()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return null;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, layer.value == 0 ? Physics2D.DefaultRaycastLayers : layer);
        if (hit == null)
            hit = Physics2D.OverlapPoint(mouseWorldPos);

        return GetHumanFromCollider(hit);
    }

    private Human GetHumanFromCollider(Collider2D collider)
    {
        if (collider == null)
            return null;

        Human human = collider.GetComponent<Human>();
        if (human == null)
            human = collider.GetComponentInParent<Human>();

        return human;
    }

    private bool IsCorrectFlyerForHuman(Human human, string selectedFlyer)
    {
        return human != null && human.needReaction != null && human.needReaction.Length > 0 && selectedFlyer == human.needReaction[0];
    }

    private Sprite GetBubbleSprite(string selectedFlyer)
    {
        if (flyerVisualDatabase == null)
            return null;

        return flyerVisualDatabase.GetBubbleSprite(selectedFlyer);
    }

    private void TryAddPoliceSuspicion(Human human, string selectedFlyer)
    {
        if (!isPolice || human == null)
            return;

        // Проверяем не только флаг OnTriggerEnter2D, но и реальное попадание в активную зону полиции.
        // ВАЖНО: считается зона ЦЕЛИ, в которую игрок кинул листовку, а не позиция культиста и не траектория листовки.
        bool isInsidePoliceZone = PoliceZone.IsHumanInsideAnyZone(human);

        if (suspicionOnlyInsidePoliceZone && !isInsidePoliceZone)
            return;

        // Новая понятная система полиции:
        // любая применённая листовка в зоне полиции = +1 предупреждение, даже если листовка правильная.
        // Мегафон дополнительных предупреждений не добавляет, потому что здесь считается только основной ручной бросок.
        currentSuspicion += 1f;

        int catchLimit = GetCurrentSuspicionLimit();
        currentSuspicion = Mathf.Clamp(currentSuspicion, 0f, catchLimit);

        if (GameSessionBridge.Instance != null)
            GameSessionBridge.Instance.RegisterSuspicion(currentSuspicion);

        Debug.Log("Полиция заметила листовку в зоне: предупреждение " + currentSuspicion + "/" + catchLimit);
        UpdateSuspicionUI();

        if (currentSuspicion >= catchLimit)
            OnMaxSuspicionReached();
    }

    private int GetCurrentSuspicionLimit()
    {
        int limit = Mathf.Max(1, wrongFlyersToCatch);

        if (HasSelfImprovementClubActive)
            limit += Mathf.Max(0, selfImprovementExtraWrongFlyers);

        return Mathf.Max(1, limit);
    }

    private void OnMaxSuspicionReached()
    {
        if (HasDevilAdvocateActive && !devilAdvocateUsedToday)
        {
            devilAdvocateUsedToday = true;

            if (GameSessionBridge.Instance != null)
                GameSessionBridge.Instance.SetDevilAdvocateUsedToday(true);

            currentSuspicion = Mathf.Max(0f, currentSuspicion - Mathf.Max(1, devilAdvocateReduceWrongFlyers));
            Debug.Log(devilAdvocateMessage + ": " + currentSuspicion + "/" + GetCurrentSuspicionLimit());
            UpdateSuspicionUI();

            // Визуально показываем, что полиция почти поймала игрока,
            // но адвокат дьявола отбил задержание: ближайшая полиция добегает до точки
            // и продолжает идти дальше к выходу. День НЕ заканчивается, спавн и таймер НЕ блокируются.
            PoliceController.StartClosestDevilAdvocateBlockRun(playerTarget);
            return;
        }

        streetLocked = true;
        HideMegafonPreview();

        if (dayController != null)
            dayController.StopTimerOnly();

        if (spawnHuman != null)
            spawnHuman.StopNewSpawnsOnly();

        if (GameSessionBridge.Instance != null)
            GameSessionBridge.Instance.SetArrestHappenedToday(true);

        bool chaseStarted = PoliceController.StartClosestChase(playerTarget, OnPoliceCaughtPlayer);
        if (!chaseStarted)
            OnPoliceCaughtPlayer();
    }

    private void OnPoliceCaughtPlayer()
    {
        if (dayController != null)
            dayController.ForceEndDay(policeCatchReason);
    }

    private void UpdateSuspicionUI()
    {
        int catchLimit = GetCurrentSuspicionLimit();
        float normalized = catchLimit <= 0 ? 0f : Mathf.Clamp01(currentSuspicion / catchLimit);
        string text = suspicionPrefix + Mathf.RoundToInt(currentSuspicion) + " / " + catchLimit;

        if (suspicionText != null)
            suspicionText.text = text;

        if (suspicionTMPText != null)
            suspicionTMPText.text = text;

        if (suspicionFillImage != null)
            suspicionFillImage.fillAmount = normalized;
    }

    public void SetMegafonActive(bool value) { isMegafon = value; }
    public void SetDevilAdvocateActive(bool value) { hasDevilAdvocate = value; }
}
