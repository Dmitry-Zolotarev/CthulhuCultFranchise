using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "FlyerVisualDatabase", menuName = "Cthulhu/Flyer Visual Database")]
public class FlyerVisualDatabase : ScriptableObject
{
    [System.Serializable]
    public class FlyerVisual
    {
        [Header("ID from FlyersController: worker2 / student2 / retiree2 / blogger2 / esoteric2")]
        public string flyerId;

        [Header("ONE sprite for this flyer. The same shared FlyingFlyer prefab will use this sprite at runtime.")]
        [FormerlySerializedAs("openSprite")]
        public Sprite flyerSprite;

        [Header("Optional: icon shown inside thought bubble. If empty, flyerSprite is used.")]
        public Sprite bubbleSprite;
    }

    [SerializeField] private FlyerVisual[] flyers;

    public Sprite GetFlyerSprite(string flyerId)
    {
        FlyerVisual visual = FindVisual(flyerId);
        return visual != null ? visual.flyerSprite : null;
    }

    public Sprite GetBubbleSprite(string flyerId)
    {
        FlyerVisual visual = FindVisual(flyerId);
        if (visual == null)
            return null;

        return visual.bubbleSprite != null ? visual.bubbleSprite : visual.flyerSprite;
    }

    private FlyerVisual FindVisual(string flyerId)
    {
        if (flyers == null)
            return null;

        for (int i = 0; i < flyers.Length; i++)
        {
            if (flyers[i] != null && flyers[i].flyerId == flyerId)
                return flyers[i];
        }

        return null;
    }
}
