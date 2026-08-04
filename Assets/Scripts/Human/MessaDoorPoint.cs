using UnityEngine;

// Поставь этот скрипт на пустой объект у двери церкви.
// Правильные прохожие будут идти к этому объекту.
public class MessaDoorPoint : MonoBehaviour
{
    public static Transform Current { get; private set; }

    private void Awake()
    {
        Current = transform;
    }

    private void OnDestroy()
    {
        if (Current == transform)
            Current = null;
    }
}
