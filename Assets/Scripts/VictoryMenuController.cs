using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VictoryMenuController : MonoBehaviour
{
    public static float currentLevelTime; // completion time of latest run for current level (after victory)
    [SerializeField] private TextMeshProUGUI levelTimeText; // GUI
    [SerializeField] private TextMeshProUGUI levelRecordText; // GUI
    [SerializeField] private TextMeshProUGUI levelNewRecordTimeText; // GUI

    private void Start()
    {
        int currentLevelIndex = MenuController.currentLevel - MenuController.firstLevel;
        float levelRecordTime = PlayerPrefs.GetFloat($"Level {currentLevelIndex} Record", 0f);

        levelTimeText.text = $"Level {currentLevelIndex} Time:\t{Misc.FormatSeconds(currentLevelTime)}";
        levelRecordText.text = $"Level {currentLevelIndex} Record:\t{Misc.FormatSeconds(levelRecordTime)}";

        if (currentLevelTime == levelRecordTime)
        {
            levelNewRecordTimeText.text = "New Record Time!";
        }
        else
        {
            levelNewRecordTimeText.text = "";
        }
    }
}
