using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelTimeController : MonoBehaviour
{
    private float levelTime;
    private string levelTimeRecordName;
    [SerializeField] private TextMeshProUGUI levelTimeUI; // GUI

    private void Start()
    {
        // assign event listeners
        PlayerController.PlayerVictoryEvent += SaveLevelTime;

        levelTimeRecordName = $"{SceneManager.GetActiveScene().name} Record";

        levelTime = 0f;
    }

    // update level time
    private void Update()
    {
        if (!PlayerController.victory)
        {
            levelTime += Time.deltaTime;
            string formattedTime = Misc.FormatSeconds(levelTime);
            levelTimeUI.text = formattedTime.Substring(0, formattedTime.Length - 3) + "s"; // formatted time cut off at tens place
        }
    }

    // save latest time, update record
    private void SaveLevelTime()
    {
        VictoryMenuController.currentLevelTime = levelTime;

        float levelTimeRecord = PlayerPrefs.GetFloat(levelTimeRecordName, float.MaxValue);
        if (levelTime < levelTimeRecord)
        {
            PlayerPrefs.SetFloat(levelTimeRecordName, levelTime); // playerprefs saving ftw
            PlayerPrefs.Save();
        }
    }
}
