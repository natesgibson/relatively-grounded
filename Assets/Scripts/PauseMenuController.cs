using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu; // GUI
    [SerializeField] private GameObject playerTouchControls; // GUI
    [SerializeField] private GameObject touchControlsToggle; // GUI
    [SerializeField] private GameObject levelTime; // GUI
    [SerializeField] private GameObject levelTimeToggle; // GUI
    [SerializeField] private GameObject keyboardControlsText; // GUI

    // tracks if game is paused
    private bool gamePaused;
    
    private void Start()
    {
        // assign event listeners
        PlayerController.PlayerVictoryEvent += UIOff;

        // start game not paused
        gamePaused = false;
        pauseMenu.SetActive(false);

        // load levelTimeOn PlayerPrefs setting
        bool levelTimeOnPref = PlayerPrefs.GetString("levelTimeOn", "True") == "True";
        levelTimeToggle.GetComponent<Toggle>().isOn = levelTimeOnPref;
        levelTime.SetActive(levelTimeOnPref);

        // mobile: touch controls on and disable toggle
        if (Application.platform == RuntimePlatform.Android  || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            playerTouchControls.SetActive(true);
            PlayerPrefs.SetString("touchControlsOn", "True");
            PlayerPrefs.Save();
            touchControlsToggle.SetActive(false);
            keyboardControlsText.SetActive(false);
            Application.targetFrameRate = 60; // TODO: move somewhere more appropriate
        }
        // other: load touchControlsOn PlayerPrefs setting
        else
        {
            bool touchControlsOnPref = PlayerPrefs.GetString("touchControlsOn", "False") == "True"; // string to bool conversion
            touchControlsToggle.GetComponent<Toggle>().isOn = touchControlsOnPref;
            playerTouchControls.SetActive(touchControlsOnPref);
            keyboardControlsText.SetActive(true);
        }
    }

    private void Update()
    {
        // toggle pause based on user input
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) && !PlayerController.victory)
        {
            if (!gamePaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    // pause game: show pause menu and pause physics
    public void PauseGame()
    {
        gamePaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        playerTouchControls.SetActive(false);
    }

    // resume game: hide pause menu and resume physics
    public void ResumeGame()
    {
        gamePaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        playerTouchControls.SetActive(PlayerPrefs.GetString("touchControlsOn", "False") == "True");
    }

    // load scene by name (assigned in GUI)
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        ResumeGame();
    }

    // reload current scene
    public void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    // toggles touch controls and saves in playerprefs
    public void ToggleTouchControls()
    {
        bool touchControlsToggleOn = touchControlsToggle.GetComponent<Toggle>().isOn;
        PlayerPrefs.SetString("touchControlsOn", touchControlsToggleOn.ToString());
        PlayerPrefs.Save();
    }

    // toggles display level time and saves in playerprefs
    public void ToggleLevelTime()
    {
        bool levelTimeOn = levelTimeToggle.GetComponent<Toggle>().isOn;
        levelTime.SetActive(levelTimeOn);
        PlayerPrefs.SetString("levelTimeOn", levelTimeOn.ToString());
        PlayerPrefs.Save();
    }

    private void UIOff()
    {
        playerTouchControls.SetActive(false);
        levelTime.SetActive(false);
    }
}
