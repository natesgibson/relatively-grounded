using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawController : MonoBehaviour
{
    // saw movement parameters
    [SerializeField] private Transform ground; // GUI
    private Vector3 startPosition;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 2f;
    public bool startReversed = false;

    private void Start()
    {
        // if ground is assigned and saw circle child exists, change the saw circle color to match ground
        if (ground != null)
        {
            Transform sawCircle = transform.Find("Saw Circle"); // (only searches children)
            if (sawCircle != null)
            {
               sawCircle.GetComponent<SpriteRenderer>().color = ground.GetComponent<SpriteRenderer>().color;
            }
        }

        // save starting position of saw
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // move saw perpendicular to ground in ping-pong pattern at custom speed and distance
        if (ground != null && speed != 0f && distance > 0f)
        {
            Vector3 deltaPosition = ((Mathf.PingPong(Time.time*speed, distance) - (distance/2)) * ground.right.normalized);

            if (startReversed)
            {
                transform.position = startPosition + deltaPosition;
            }
            else
            {
                transform.position = startPosition - deltaPosition;
            }
        }
    }
}
