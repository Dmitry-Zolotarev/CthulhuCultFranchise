using UnityEngine;


public class ScreenManager : MonoBehaviour
{
    public GameObject[] Menus;
    public static ScreenManager Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public void OpenMenu(int menuID)
    {
        Time.timeScale = 1f;
        for (int i = 0; i < Menus.Length; i++) Menus[i]?.SetActive(i == menuID);
    }
    public void CloseMenus()
    {
        foreach (var menu in Menus) menu?.SetActive(false);
    }
}
