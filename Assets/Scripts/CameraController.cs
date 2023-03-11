using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // camera position
    private Transform player;
    private int cameraZPosition = -10;

    // camera rotation
    private bool rotate = false;
    private float rotationSpeed = 0.5f;

    // camera rotate relative to player
    private bool rotateWithPlayer = false;
    private bool rotateOppositePlayer = false;
    private Quaternion cameraInitialRotation;
    private Quaternion playerInitialRotation;
    

    private void Start()
    {
        // assign event listeners
        PlayerController.PlayerKeysChangedEvent += UpdateCameraRotation;

        // assign player transform
        player = GameObject.FindWithTag("Player").transform;
    }

	private void LateUpdate() 
	{
        // camera follow player
        gameObject.transform.position = new Vector3(player.position.x, player.position.y, cameraZPosition);

        // camera rotates with player
        if (rotateWithPlayer && !PlayerController.victory)
        {
            this.transform.rotation = cameraInitialRotation * player.rotation * Quaternion.Inverse(playerInitialRotation);
            
        }
        // camera rotates opposite of player
        if (rotateOppositePlayer && !PlayerController.victory)
        {
            this.transform.rotation = cameraInitialRotation * playerInitialRotation * Quaternion.Inverse( player.rotation);
        }
	}

    private void FixedUpdate()
    {
        // camera rotate at rotation speed
        if (rotate && !PlayerController.victory)
        {
            this.transform.Rotate(0, 0, rotationSpeed);
        }
    }

    // modify camera rotation based on number of player keys collected
    private void UpdateCameraRotation()
    {
        int playerKeysCount = player.GetComponent<PlayerController>().collectedKeys.Count;
        // 0 keys: no rotation
        if (playerKeysCount == 0)
        {
            rotateWithPlayer = false;
            rotate = false;
        }
        // 1-2 keys: rotate faster with each key
        else if (playerKeysCount < 3)
        {
            rotateWithPlayer = false;
            rotate = true;
            rotationSpeed = playerKeysCount * Random.Range(0.2f, 0.3f) * (Random.value < 0.5? 1f : -1f); // stochastic magnitude and direction
        }
        else
        {
            // 3+ keys: alternate rotate with and opposite player
            cameraInitialRotation = transform.rotation;
            playerInitialRotation = player.rotation;
            if (rotateWithPlayer)
            {
                rotate = false;
                rotateWithPlayer = false;
                rotateOppositePlayer = true;
            }
            else
            {
                rotate = false;
                rotateWithPlayer = true;
                rotateOppositePlayer = false;
            }
        }
    }
}