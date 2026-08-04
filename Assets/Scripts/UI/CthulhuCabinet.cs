using UnityEngine;

public class CthulhuCabinet : MonoBehaviour
{
    [SerializeField] private AudioClip CthulhuAmbient;
    private void Start()
    {
        MusicPlayer.Instance?.PlayMusic(CthulhuAmbient);
    }
    private void OnEnable()
    {
        MusicPlayer.Instance?.PlayMusic(CthulhuAmbient);
    }
}
