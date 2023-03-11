using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misc : MonoBehaviour
{
    // hide mouse variables
    [SerializeField] private float hideMouseDelay;
    private Coroutine hideMouseCoroutine;

    private void Start()
    {
        hideMouseCoroutine = null;

        // change mobile framerate settings
        if (Application.platform == RuntimePlatform.Android  || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Application.targetFrameRate = 60;
        }
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0)
        {
            if (hideMouseCoroutine == null)
            {
                hideMouseCoroutine = StartCoroutine(HideMouse());
            }
        }
        else
        {
            if (hideMouseCoroutine != null)
            {
                StopCoroutine(hideMouseCoroutine);
                hideMouseCoroutine = null;
                Cursor.visible = true;
            }
        }
    }

    // hides mouse after duration of inactivity
    private IEnumerator HideMouse()
    {
        yield return new WaitForSeconds(hideMouseDelay);
        Cursor.visible = false;
    }
    
    // takes float of seconds and returns string with format "(HHh) (MMm) SS.DDDs"
    public static string FormatSeconds(float seconds)
    {   
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        string formattedTime = "";

        if (time.Hours > 0)
        {
            formattedTime += $"{time.Hours.ToString()}h ";
        }
        if (time.Minutes > 0)
        {
            formattedTime += $"{time.Minutes.ToString()}m ";
        }
        formattedTime += $"{(time.Seconds + (time.Milliseconds/1000f)).ToString("0.000")}s";

        return formattedTime;
    }
}
