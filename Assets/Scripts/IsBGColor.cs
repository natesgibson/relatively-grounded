using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsBGColor : MonoBehaviour
{
    // transparency parameter
    [SerializeField] private float transparency = 1f;

    private void Start()
    {
        // set object color to background color of main camera, with custom transparency
        Vector4 bgColor = GameObject.FindWithTag("MainCamera").GetComponent<Camera>().backgroundColor;
        gameObject.GetComponent<SpriteRenderer>().color = new Vector4(bgColor.x, bgColor.y, bgColor.z, transparency);
    }
}
