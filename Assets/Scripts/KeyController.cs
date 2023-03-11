using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    // key components
    private CircleCollider2D keyCC;
    private ParticleSystem keyPS;

    // tracks if key has been used to unlock a gate
    public bool keyUsed;

    private void Start()
    {
        // assign event listeners
        PlayerController.PlayerRespawnEvent += ResetKeys;

        // assign key components
        keyCC = gameObject.GetComponent<CircleCollider2D>();
        keyPS = gameObject.GetComponent<ParticleSystem>();

        // initialize key
        keyUsed = false;
        ResetKeys();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    { 
        // if key is triggered by player, disable collision and make invisible
        if (collider.gameObject.CompareTag("Player"))
        {
            keyCC.enabled = false;
            keyPS.Stop();
        }
    }

    // if key has not been used to unlock a gate,
    // reset key so it can be triggered by player and is visible
    private void ResetKeys()
    {
        if (!keyUsed)
        {
            keyCC.enabled = true;
            keyPS.Play();
        }
    }
}
