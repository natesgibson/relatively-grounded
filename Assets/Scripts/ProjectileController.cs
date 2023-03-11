using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public Vector3 spVelocity; // set by gun when projectile is instantiated

    private void FixedUpdate()
    {
        // move!
        transform.position += new Vector3(spVelocity.x, spVelocity.y, 0) * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // projectile touches player: rip (both)
        if (collider.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        // projectile goes out of bounds: rip
        if (collider.gameObject.CompareTag("Bounds"))
        {
            Destroy(gameObject);
        }
    }
}
