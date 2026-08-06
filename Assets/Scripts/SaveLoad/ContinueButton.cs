using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ContinueButton : MonoBehaviour
{
    private Button continueButton;

    private void Awake()
    {
        continueButton = GetComponent<Button>();
        continueButton.interactable = SaveManager.SaveExists();
    }
    private void Start()
    {
        MusicPlayer.Instance.PlayMainMusic();
    }
    public void Continue()
    {
        SaveManager.NeedLoad = true;
        SceneManager.LoadScene(1);
    }
}
