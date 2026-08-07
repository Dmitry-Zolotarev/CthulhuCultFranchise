using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{  
    [SerializeField] private AudioClip defaultMusic;
    [HideInInspector] public AudioSource AudioSource;

    public static MusicPlayer Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        AudioSource = GetComponent<AudioSource>();
        AudioSource.loop = true;
        PlayDefaultMusic();
    }
    public void PlayMusic(AudioClip clip)
    {
        if (clip != AudioSource.clip)
        {
            AudioSource.clip = clip;
            AudioSource.Play();
        } 
    }
    public void PlayDefaultMusic()
    {
        PlayMusic(defaultMusic);
    }
}