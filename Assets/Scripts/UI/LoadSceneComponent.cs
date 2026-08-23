using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneComponent : MonoBehaviour
{
    public void LoadScene(int sceneID)
    {       
        Time.timeScale = 1f;
        if (sceneID == 0) SaveManager.Save();
        MusicPlayer.Instance?.PlayDefaultMusic();
        SceneManager.LoadScene(sceneID);
    }
}