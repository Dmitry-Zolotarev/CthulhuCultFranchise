using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioData[] sounds;
    [HideInInspector] public AudioSource AudioSource;
    [HideInInspector] public static SFXPlayer Instance;
    
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
    
    public void Play(string id)
    {
        foreach (var audioData in sounds)
        {
            if (audioData.id == id)
            {
                AudioSource.PlayOneShot(audioData.clip);
                break;
            }
        }
    }
    public void Stop() => AudioSource.Stop();

    [System.Serializable]
    public class AudioData
    {
        [SerializeField] private string _id;
        [SerializeField] private AudioClip _clip;
        public string id => _id;
        public AudioClip clip => _clip;
    }
}
