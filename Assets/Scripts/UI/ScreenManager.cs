using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public GameObject[] Menus;
    
    private void OnEnable()
    {
        OpenMenu(0);        
    }
    public void OpenMenu(int canvasID)
    {
        for (int i = 0; i < Menus.Length; i++) Menus[i]?.SetActive(i == canvasID);
        GameManager.Instance.UpdateUI();
    }
}
