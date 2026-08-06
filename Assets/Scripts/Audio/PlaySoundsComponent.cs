using UnityEngine;

public class PlaySoundsComponent : MonoBehaviour
{
    [SerializeField] private AudioClip sound;
    private float lastPlayTime;

    public void Play()
    {
        if (sound != null && Time.time > lastPlayTime)
        {
            lastPlayTime = Time.time;
            SFXPlayer.Instance.AudioSource?.PlayOneShot(sound);
        }
    }
}
