using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    //* Attach this script to the UserInterface game object.

    const KeyCode pressingPlayKey1 = KeyCode.Space, pressingPlayKey2 = KeyCode.Return;
    bool passedGameNameText;
    [SerializeField] GameObject gameNameText, playButton, howToPlayText, explaining1, explaining2, explaining3, explaining4, explaining5, okButton;

    void Start()
    {
        QualitySettings.vSyncCount = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(pressingPlayKey1) || Input.GetKeyUp(pressingPlayKey2))
        {
            if (!passedGameNameText)
            {
                PlayButton();
            }
            else
            {
                OkButton();
            }
        }
    }

    public void PlayButton()
    {
        gameNameText.SetActive(false);
        playButton.SetActive(false);
        howToPlayText.SetActive(true);
        explaining1.SetActive(true);
        explaining2.SetActive(true);
        explaining3.SetActive(true);
        explaining4.SetActive(true);
        explaining5.SetActive(true);
        okButton.SetActive(true);
        passedGameNameText = true;
    }

    public void OkButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
