using TMPro;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    //* Attach this script to the UserInterface game object.

    public static bool gamePaused;
    const KeyCode escapeKey = KeyCode.Escape;
    [SerializeField] GameObject pauseMenuObject;
    [SerializeField] TextMeshProUGUI gameVersionText;

    void Start()
    {
        gameVersionText.text = $"v{Application.version}";
    }

    void Update()
    {
        if (Input.GetKeyDown(escapeKey))
        {
            if (!gamePaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }

        Cursor.lockState = !gamePaused ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = gamePaused;
    }

    void Pause()
    {
        pauseMenuObject.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
    }

    public void Resume()
    {
        pauseMenuObject.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    public void QuittingGame()
    {
        Application.Quit();
    }
}
