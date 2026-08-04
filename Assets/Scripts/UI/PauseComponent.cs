using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseComponent : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject cthulhuCabinet;

    [SerializeField] private GameObject pauseMenuPrefab;

    private GameObject pauseMenu;
    private OptionsMenu optionsMenu;

    public static PauseComponent Instance;

    private void Awake()
    {
        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Time.timeScale = 1f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mainMenu = GameObject.Find("MainMenu");
        cthulhuCabinet = GameObject.Find("CthulhuCabinet");

        // пересоздаём pause menu
        if (pauseMenu != null)
            Destroy(pauseMenu);

        if (pauseMenuPrefab != null)
        {
            pauseMenu = Instantiate(pauseMenuPrefab);
            optionsMenu = pauseMenu.GetComponentInChildren<OptionsMenu>();
            pauseMenu.SetActive(false);
        }
    }

    public void Pause()
    {
        if (mainMenu != null && mainMenu.activeSelf) return;
        if (cthulhuCabinet != null && cthulhuCabinet.activeSelf) return;

        if (pauseMenu == null) return;


        if (optionsMenu != null)
            optionsMenu.gameObject.SetActive(false);

        if (pauseMenu.activeSelf && Time.timeScale == 0f)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        else if (Time.timeScale == 1f)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Pause();
    }
}