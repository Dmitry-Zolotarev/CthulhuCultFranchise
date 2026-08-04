using System;
using System.Collections.Generic;
using UnityEngine;

// Визуальный переключатель улучшений на сцене.
// Читает активные чекбоксы из GameSessionBridge и включает/выключает декоративные объекты.
// Например: куплен мегафон -> включить модель/иконку мегафона на храме; куплен адвокат -> включить табличку адвоката.
public class UpgradeVisualActivator : MonoBehaviour
{
    public enum UpgradeType
    {
        Megafon,
        DevilAdvocate,
        SelfImprovementClub,
        PremiumFlyer,
        CthulhuMerch,
        WordOfMouth,
        AbyssAccountant,
        Cookies,
        Altar,
        Candles,
        PaidFrontRow,
        Choir
    }

    [Serializable]
    public class UpgradeVisualBinding
    {
        public UpgradeType upgradeType;
        public GameObject visualObject;
        [Tooltip("ON = объект будет выключен, когда улучшение активно. Обычно оставь OFF.")]
        public bool invert;
    }

    [Header("Bridge")]
    [SerializeField] private GameSessionBridge bridge;
    [SerializeField] private bool autoFindBridge = true;

    [Header("Refresh")]
    [Tooltip("ON = обновлять каждый кадр. Удобно для отладки чекбоксов в инспекторе. Для релиза можно выключить и вызывать RefreshVisuals() после покупки улучшения.")]
    [SerializeField] private bool refreshEveryFrame = true;
    [SerializeField] private bool refreshOnEnable = true;

    [Header("Visual bindings")]
    [SerializeField] private List<UpgradeVisualBinding> visuals = new List<UpgradeVisualBinding>();

    private void OnEnable()
    {
        if (refreshOnEnable)
            RefreshVisuals();
    }

    private void Update()
    {
        if (refreshEveryFrame)
            RefreshVisuals();
    }

    public void RefreshVisuals()
    {
        GameSessionBridge targetBridge = GetBridge();
        if (targetBridge == null)
            return;

        for (int i = 0; i < visuals.Count; i++)
        {
            UpgradeVisualBinding binding = visuals[i];
            if (binding == null || binding.visualObject == null)
                continue;

            bool active = IsUpgradeActive(targetBridge, binding.upgradeType);
            if (binding.invert)
                active = !active;

            if (binding.visualObject.activeSelf != active)
                binding.visualObject.SetActive(active);
        }
    }

    public void ForceAllOff()
    {
        for (int i = 0; i < visuals.Count; i++)
        {
            if (visuals[i] != null && visuals[i].visualObject != null)
                visuals[i].visualObject.SetActive(false);
        }
    }

    private GameSessionBridge GetBridge()
    {
        if (bridge == null && autoFindBridge)
            bridge = GameSessionBridge.Instance;

        if (bridge == null && autoFindBridge)
            bridge = FindObjectOfType<GameSessionBridge>();

        return bridge;
    }

    private bool IsUpgradeActive(GameSessionBridge targetBridge, UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.Megafon:
                return targetBridge.HasMegafon;
            case UpgradeType.DevilAdvocate:
                return targetBridge.HasDevilAdvocate;
            case UpgradeType.SelfImprovementClub:
                return targetBridge.HasSelfImprovementClub;
            case UpgradeType.PremiumFlyer:
                return targetBridge.HasPremiumFlyer;
            case UpgradeType.CthulhuMerch:
                return targetBridge.HasCthulhuMerch;
            case UpgradeType.WordOfMouth:
                return targetBridge.HasWordOfMouth;
            case UpgradeType.AbyssAccountant:
                return targetBridge.HasAbyssAccountant;
            case UpgradeType.Cookies:
                return targetBridge.HasCookies;
            case UpgradeType.Altar:
                return targetBridge.HasAltar;
            case UpgradeType.Candles:
                return targetBridge.HasCandles;
            case UpgradeType.PaidFrontRow:
                return targetBridge.HasPaidFrontRow;
            case UpgradeType.Choir:
                return targetBridge.HasChoir;
            default:
                return false;
        }
    }
}
