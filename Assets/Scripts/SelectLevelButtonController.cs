using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectLevelButtonController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelRecordText; // GUI
    [SerializeField] private Image levelThumbnail; // GUI

    private void Start()
    {
        int levelIndex = Int32.Parse(gameObject.name.Split(" ")[1]);

        // level unlocked
        if (levelIndex <= MenuController.farthestLevel - MenuController.firstLevel)
        {
            gameObject.GetComponent<Button>().interactable = true;
            levelThumbnail.color = new Vector4(1f, 1f, 1f, 1f);
            float levelRecordTime = PlayerPrefs.GetFloat($"Level {levelIndex} Record", 0);
            levelRecordText.text = levelRecordTime > 0 ? $"Record Time:\n{Misc.FormatSeconds(levelRecordTime)}" : "";
        }

        // level locked
        else
        {
            gameObject.GetComponent<Button>().interactable = false;
            levelThumbnail.color = new Vector4(1f, 1f, 1f, 0.3f);
            levelRecordText.text = "";
        }
    }
}
