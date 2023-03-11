using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawRotate : MonoBehaviour
{
    // saw rotation parameters
    [SerializeField] private float rotationSpeed = 3.5f;

    private void FixedUpdate()
    {
        // rotate saw at rotationSpeed
        transform.Rotate(0, 0, rotationSpeed);
    }
}
