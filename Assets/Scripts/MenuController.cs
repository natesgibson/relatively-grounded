using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static int firstLevel = 4; // first level of game [MUST MANUALLY UPDATE]
    public static int lastLevel = 14; // last level of game [MUST MANUALLY UPDATE]
    public static int farthestLevel = firstLevel; // furthest unlocked level
    public static int currentLevel = firstLevel; // last level played

    private void Awake()
    {
        // load the farthest level when opening the menu
        farthestLevel = PlayerPrefs.GetInt("farthestLevel", firstLevel); // playerprefs saving ftw
    }

    // load scene by name (assigned in GUI)
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // continue game from farthest level
    public void Continue()
    {
        SceneManager.LoadScene(farthestLevel);
    }

    // load next level
    public void NextLevel()
    {
        if (currentLevel == MenuController.lastLevel)
        {
            SceneManager.LoadScene("UltimateVictory");
        }
        else
        {
            int nextLevelIndex = currentLevel + 1;
            SceneManager.LoadScene(nextLevelIndex);
        }
    }

    // replay last level
    public void ReplayLevel()
    {
        SceneManager.LoadScene(currentLevel);
    }

    // quit the game
    public void ExitGame()
    {
        Application.Quit();
    }
}
