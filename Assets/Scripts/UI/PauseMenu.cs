using UnityEngine;
public class PauseMenu : MonoBehaviour
{
    public void Resume()
    {
        PauseComponent.Instance?.Pause();
    }
    public void ExitToMenu()
    {
        GameSessionBridge.Instance?.OpenMainMenu();
    }
}
