using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectedGroundController : MonoBehaviour
{
    [SerializeField] private bool antiGravity = false;
    [SerializeField] private float antiGravityMagnitude;
    [SerializeField] private Vector3 customGravity;

    // totally reject player
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().ResetTouching();

            if (antiGravity)
            {
                Physics2D.gravity = -collision.contacts[0].normal * antiGravityMagnitude;
            }
            else
            {
                Physics2D.gravity = customGravity;
            }
        }
    }
}
