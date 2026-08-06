using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [HideInInspector] public static SFXPlayer Instance;
    [HideInInspector] public AudioSource AudioSource;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        AudioSource = GetComponent<AudioSource>();
    }
}
