using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PoliceZone : MonoBehaviour
{
    private static readonly List<PoliceZone> activeZones = new List<PoliceZone>();

    [Header("Visual")]
    [Tooltip("Опционально. Полупрозрачный круг зоны полиции.")]
    [SerializeField] private SpriteRenderer zoneRenderer;
    [Range(0f, 1f)]
    [SerializeField] private float visibleAlpha = 0.25f;

    private Collider2D zoneCollider;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
        zoneRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        zoneCollider = GetComponent<Collider2D>();
        zoneCollider.isTrigger = true;

        if (zoneRenderer == null)
            zoneRenderer = GetComponent<SpriteRenderer>();

        ApplyVisualAlpha();
    }

    private void OnEnable()
    {
        if (!activeZones.Contains(this))
            activeZones.Add(this);
    }

    private void OnDisable()
    {
        activeZones.Remove(this);
    }

    private void ApplyVisualAlpha()
    {
        if (zoneRenderer == null)
            return;

        Color c = zoneRenderer.color;
        c.a = visibleAlpha;
        zoneRenderer.color = c;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Human human = other.GetComponent<Human>();
        if (human == null)
            human = other.GetComponentInParent<Human>();

        if (human != null)
            human.SetPoliceZone(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Human human = other.GetComponent<Human>();
        if (human == null)
            human = other.GetComponentInParent<Human>();

        if (human != null)
            human.SetPoliceZone(false);
    }

    public static bool IsPointInsideAnyZone(Vector2 point)
    {
        activeZones.RemoveAll(item => item == null || !item.isActiveAndEnabled);

        for (int i = 0; i < activeZones.Count; i++)
        {
            PoliceZone zone = activeZones[i];
            if (zone == null || zone.zoneCollider == null)
                continue;

            if (zone.zoneCollider.OverlapPoint(point))
                return true;
        }

        return false;
    }

    public static bool IsHumanInsideAnyZone(Human human)
    {
        if (human == null)
            return false;

        if (human.IsInPoliceZone)
            return true;

        if (IsPointInsideAnyZone(human.transform.position))
            return true;

        if (IsPointInsideAnyZone(human.GetFacePosition()))
            return true;

        return false;
    }
}
