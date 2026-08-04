using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundController : MonoBehaviour
{
    public static SoundController singletonSound { get; private set; }
    [SerializeField] private GameObject sourse2DPref;
    [SerializeField] private AudioClip[] audioClips;

    private void Start()
    {
        if (singletonSound == null)
            singletonSound = this;
        else
            Destroy(gameObject);
    }
    public void Play2DSongByIndex(int index)
    {
        if (index >= 0 && index < audioClips.Length)
            PlaySound2D(audioClips[index]);
    }

    private void PlaySound2D(AudioClip audioClip)
    {
        GameObject AudioSource2D = Instantiate(sourse2DPref, transform.position, Quaternion.identity);

        AudioSource2D.TryGetComponent(out AudioSource _audioSource);
        _audioSource.clip = audioClip;
        _audioSource.Play();

        StartCoroutine(StopSound(AudioSource2D));
    }

    
    IEnumerator StopSound(GameObject _prefSound)
    {
        _prefSound.TryGetComponent(out AudioSource _audioSource);

        yield return new WaitUntil(() => !_audioSource.isPlaying);

        Destroy(_prefSound);
    }

    public void NextScene(int id)
    {
        SceneManager.LoadScene(id);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
