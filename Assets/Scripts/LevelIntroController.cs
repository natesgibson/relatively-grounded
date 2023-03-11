using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelIntroController : MonoBehaviour
{
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private TextMeshProUGUI recordTimeText; // GUI
    [SerializeField] private GameObject levelTimeToggle; // GUI

    private void Start()
    {
        // set level intro title text to level name
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = SceneManager.GetActiveScene().name;

        // start fade animation
        StartCoroutine(Fade());

        // display record time if exists and level time ui toggle is on
        int currentLevelIndex = MenuController.currentLevel - MenuController.firstLevel;
        float currentLevelTimeRecord = PlayerPrefs.GetFloat($"Level {currentLevelIndex} Record", 0f);
        if (currentLevelTimeRecord > 0 && PlayerPrefs.GetString("levelTimeOn", "True") == "True")
        {
            recordTimeText.text = $"Record Time: {Misc.FormatSeconds(currentLevelTimeRecord)}";
        }
        else
        {
            recordTimeText.text = "";
        }
    }

    // disable gameObject after fadeTime seconds
    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(fadeTime);
        gameObject.SetActive(false);
    }
}
