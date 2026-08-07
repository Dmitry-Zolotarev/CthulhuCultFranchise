using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneComponent : MonoBehaviour
{
    public void LoadScene(int sceneID)
    {
        Time.timeScale = 1f;
        MusicPlayer.Instance.PlayDefaultMusic();
        SceneManager.LoadScene(sceneID);
    }
}