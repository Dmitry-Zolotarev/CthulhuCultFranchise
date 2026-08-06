using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{  
    [SerializeField] private AudioClip mainMusic;
    [SerializeField] private AudioClip combatMusic;
    [SerializeField] private AudioClip startMusic;
    [HideInInspector] public static MusicPlayer Instance;
    [HideInInspector] public AudioSource AudioSource;
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
        PlayMainMusic();
    }
    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (AudioSource.clip == clip && AudioSource.isPlaying) return;
        AudioSource.clip = clip;
        AudioSource.Play();
    }
    public void PlayStartMusic()
    {
        PlayMusic(startMusic);
    }
    public void PlayMainMusic()
    {
        PlayMusic(mainMusic);
    }
    public void PlayCombatMusic()
    {
        PlayMusic(combatMusic);
    }
}