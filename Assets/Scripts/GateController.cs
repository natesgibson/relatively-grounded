using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GateController : MonoBehaviour
{
    // gate components
    private SpriteRenderer gateSR;
    private BoxCollider2D gateBC;

    // PlayerController to check player collected keys count
    private PlayerController playerController;

    // keys needed to unlock gate
    public int gateKeys;

    // rejection parameters
    [SerializeField] private Vector4 rejectionColor = new Vector4(255, 0, 0, 255); // red
    [SerializeField] private float rejectionDuration = 0.2f;
    
    // gate open parameters
    [SerializeField] private float openTransparency = 0.4f;

    private void Start()
    {
        // assign event listeners
        PlayerController.PlayerKeysChangedEvent += UpdateIsTrigger;

        // assign gate components
        gateSR = gameObject.GetComponent<SpriteRenderer>();
        gateBC = gameObject.GetComponent<BoxCollider2D>();

        // assign PlayerController
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        // intitialize gate keys
        gateKeys = gameObject.GetComponentsInChildren<Transform>().Where(child => child.CompareTag("GateKey")).Count();

        // initialize gate as unlocked
        gateBC.isTrigger = false;

        // initialize gate key particle systems as off
        foreach (ParticleSystem childPS in GetComponentsInChildren<ParticleSystem>())
        {
            childPS.Stop();
        }
    }

    // gate box collider acts as collision if player doesn't have enough keys to unlock gate
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // player is rejected
        if (collision.gameObject.CompareTag("Player"))
        {
            // start rejection animation
            StartCoroutine(RejectionAnimation());
        }
    }

    // gate box collider acts as trigger if player has enough keys to unlock gate
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // player unlocks gate
        if (collider.gameObject.CompareTag("Player"))
        {
            // change gate transparency
            gateSR.color = new Vector4(gateSR.color.r, gateSR.color.g, gateSR.color.b, openTransparency);
            // disable gate collider
            gateBC.enabled = false;

            // turn gate key particle systems on
            foreach (ParticleSystem childPS in GetComponentsInChildren<ParticleSystem>())
            {
                childPS.Play();
            }
        }
    }

    // updates if gate is trigger every time player key count changes
    private void UpdateIsTrigger()
    {
        // if player has enough keys to unlock gate -> gate collider is trigger
        if (playerController.collectedKeys.Count >= gateKeys)
        {
            gateBC.isTrigger = true;
        }
        // else player doesn't have enough keys -> gate collider is not trigger
        else
        {
            gateBC.isTrigger = false;
        }
    }

    // flash rejection color for duration
    private IEnumerator RejectionAnimation()
    {
        Vector4 originalColor = gateSR.color;
        gateSR.color = rejectionColor;
        yield return new WaitForSeconds(rejectionDuration);
        gateSR.color = originalColor;
    }
}
