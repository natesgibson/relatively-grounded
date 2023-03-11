using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // projectile parameters
    [SerializeField] private GameObject projectile; // GUI
    private Vector3 projectilePosition;
    private Vector4 projectileColor;

    // firing parameters
    private float timer;
    [SerializeField] private float fireFrequency = 3f;
    [SerializeField] private float fireSpeed = 5f;
    
    private void Start()
    {
        // initialize firing parameters and timer
        projectilePosition = new Vector3(transform.position.x, transform.position.y, projectile.transform.position.z);
        projectileColor = gameObject.GetComponent<SpriteRenderer>().color; // projectiles set to same color as gun
        timer = 0f;
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;

        // fire saw gun at a particular speed and frequency
        if (timer > fireFrequency)
        {
            GameObject newProjectile = Instantiate(projectile, projectilePosition, transform.rotation);
            newProjectile.GetComponent<SpriteRenderer>().color = projectileColor;
            newProjectile.GetComponent<ProjectileController>().spVelocity = transform.right.normalized * fireSpeed;
            timer = 0f;
        }
    }
}
