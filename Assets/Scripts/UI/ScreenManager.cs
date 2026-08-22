using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public GameObject[] Screens;
    public static ScreenManager Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public void OpenMenu(int menuID)
    {
        Time.timeScale = 1f;
        for (int i = 0; i < Screens.Length; i++) 
        {
            Screens[i]?.SetActive(i == menuID);
        }    
    }
}
