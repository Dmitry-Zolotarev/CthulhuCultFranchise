using UnityEngine;
using UnityEngine.UI;
public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    public static OptionsMenu Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void OnEnable()
    {
        try
        {
            musicVolumeSlider.value = MusicPlayer.Instance.AudioSource.volume;
            sfxVolumeSlider.value = SFXPlayer.Instance.AudioSource.volume;
        }
        catch { }     
    }
    public void ChangleMusicVolume()
    {
        try
        {
            MusicPlayer.Instance.AudioSource.volume = musicVolumeSlider.value;
        }
        catch { }
        
    }
    public void ChangleSoundVolume()
    {
        try
        {
            SFXPlayer.Instance.AudioSource.volume = sfxVolumeSlider.value;
        }
        catch { }    
    }
}
