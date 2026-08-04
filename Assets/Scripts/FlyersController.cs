using System;
using UnityEngine;
using UnityEngine.UI;

public class FlyersController : MonoBehaviour
{
    public static FlyersController singltoneFlyers { get; private set; }

    [Serializable]
    public class FlyerHotkey
    {
        public KeyCode key = KeyCode.Alpha1;
        public string flyerId = "worker2";
        public Sprite flyerSprite;
    }

    [Header("Current selected flyer UI")]
    [SerializeField] private Image currentFlyerIm;
    [SerializeField] private Sprite nullImage;

    [Header("Keyboard hotkeys")]
    [SerializeField] private FlyerHotkey[] hotkeys = new FlyerHotkey[]
    {
        new FlyerHotkey { key = KeyCode.Alpha1, flyerId = "worker2" },
        new FlyerHotkey { key = KeyCode.Alpha2, flyerId = "student2" },
        new FlyerHotkey { key = KeyCode.Alpha3, flyerId = "retiree2" },
        new FlyerHotkey { key = KeyCode.Alpha4, flyerId = "blogger2" },
        new FlyerHotkey { key = KeyCode.Alpha5, flyerId = "esoteric2" }
    };

    private string currentFlyer = "";

    private void Awake()
    {
        singltoneFlyers = this;
    }

    private void Update()
    {
        if (hotkeys == null)
            return;

        for (int i = 0; i < hotkeys.Length; i++)
        {
            FlyerHotkey hotkey = hotkeys[i];
            if (hotkey == null)
                continue;

            if (Input.GetKeyDown(hotkey.key))
                SelectFlyer(hotkey.flyerId, hotkey.flyerSprite);
        }
    }

    public string CurrentFlyer()
    {
        return currentFlyer;
    }

    public void SelectFlyer(string flyerId, Sprite sprite)
    {
        currentFlyer = flyerId;

        if (currentFlyerIm != null)
            currentFlyerIm.sprite = sprite != null ? sprite : nullImage;
    }

    // Оставлено для совместимости. Сейчас после броска этот метод больше НЕ вызывается.
    public void DeletFlyer()
    {
        currentFlyer = "";

        if (currentFlyerIm != null)
            currentFlyerIm.sprite = nullImage;
    }

    public void OnButtonSetFlyer(string nameFlyer)
    {
        currentFlyer = nameFlyer;
    }

    public void OnButtonSprite(Sprite im)
    {
        if (currentFlyerIm != null)
            currentFlyerIm.sprite = im;
    }
}
