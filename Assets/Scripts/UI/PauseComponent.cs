using UnityEngine;

public class PauseComponent : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject OptionsMenu;
    void Start()
    {
        if (PauseMenu != null) PauseMenu.SetActive(false);
    }
    public void Pause()
    {
        if (PauseMenu == null) return;
        
        OptionsMenu?.SetActive(false);
        if (PauseMenu.activeSelf && Time.timeScale == 0f)
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1f;  
        }
        else if(Time.timeScale == 1f)
        {
            Cursor.visible = true;
            PauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    private void FixedUpdate()
    {
        if (PauseMenu == null) PauseMenu = GameObject.FindGameObjectWithTag("PauseCanvas");
        if (OptionsMenu == null) OptionsMenu = GameObject.FindGameObjectWithTag("OptionsMenu");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Pause();
    }
}
