using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// keyboard controls: left arrow -> rotate counterclockwise | right arrow -> rotate clockwise | space -> jump | escape / p -> pause game

public class PlayerController : MonoBehaviour
{
    // custom events
    public static event Action PlayerRespawnEvent;
    public static event Action PlayerVictoryEvent;
    public static event Action PlayerKeysChangedEvent; // (should be invoked every time player key collection changes)

    // player components
    private SpriteRenderer playerSR;
    private Rigidbody2D playerRB;
    private CircleCollider2D playerCC;
    private ParticleSystem playerKeyPS;

    // ordered dictionary of ground objects player is touching
    // key: ground id, value: gravity
    private OrderedDictionary touching;

    // queue of keys player has collected
    public Queue<GameObject> collectedKeys;

    // player input bools
    public static bool rotate_counter;
    public static bool rotate_clock;
    public static bool jump;

    // player physics constants
    [SerializeField] private float gravityMagnitude = 10f;
    [SerializeField] private float forceMagnitude = 0.3f;
    [SerializeField] private float jumpMagnitude = 5f;
    [SerializeField] private float maxVelocity = 10f;

    // tracks if player is jumping
    private bool jumping;

    // spawn and respawn parameters
    private Vector3 spawnPosition;
    [SerializeField] private Vector3 spawnGravity = new Vector2(0, -25);
    private float spawnDuration = 0.5f;
    private float respawnDuration = 0.5f;
    private bool respawning;

    // gate rejection parameter
    private float gateRejectionStrength = 10f;

    // particle systems for annimations
    [SerializeField] private GameObject spawnPS; // GUI
    [SerializeField] private GameObject deathPS; // GUI

    // player key particle system constants
    private float playerKeySize = 0.7f;
    private float playerKeySizeIncrement = 0.2f;

    // tracks if player has achieved Victory!
    [NonSerialized] public static bool victory;
    private float playerVictoryDuration = 3f;

    private void Awake()
    {
        // reset public static events
        PlayerRespawnEvent = null;
        PlayerVictoryEvent = null;
        PlayerKeysChangedEvent = null;

        // set current level for menus
        MenuController.currentLevel = SceneManager.GetActiveScene().buildIndex;
    }

    private void Start()
    {
        // assign event listeners
        PlayerKeysChangedEvent += UpdatePlayerKeyPS;

        // assign player components
        playerSR = this.GetComponent<SpriteRenderer>();
        playerRB = this.GetComponent<Rigidbody2D>();
        playerCC = this.GetComponent<CircleCollider2D>();
        playerKeyPS = transform.GetChild(0).GetComponent<ParticleSystem>();

        // initialize player input bools
        rotate_counter = false;
        rotate_clock = false;
        jump = false;

        // enable player jump
        jumping = false;

        // initialize spawn location
        spawnPosition = transform.position;

        // change particle system colors to match player
        ParticleSystem.MinMaxGradient playerColorMMG = new ParticleSystem.MinMaxGradient(playerSR.color);
        ParticleSystem.MainModule psSettings = spawnPS.GetComponent<ParticleSystem>().main;
        psSettings.startColor = playerColorMMG;
        psSettings = deathPS.GetComponent<ParticleSystem>().main;
        psSettings.startColor = playerColorMMG;

        // Victory! is not a guarantee
        victory = false;

        // spawn player
        PlayerSpawn();
    }

    // update player input bools based on user input
    private void Update()
    {
        rotate_counter = Input.GetKey(KeyCode.LeftArrow) || TouchButtonInputs.uiPlayerCounter;
        rotate_clock = Input.GetKey(KeyCode.RightArrow) || TouchButtonInputs.uiPlayerClock;
        jump = Input.GetKey(KeyCode.Space) || TouchButtonInputs.uiPlayerJump;
    }

    // updates player physics based on player input bools
    private void FixedUpdate()
    {
        if (touching.Count > 0 && !respawning)
        {
            // jump
            if (jump && !jumping)
            {
                jumping = true;
                // set gravity based on last ground touched (for consistent corner behavior)
                Physics2D.gravity = (Vector2)touching[0];
                // apply jump velocity
                playerRB.velocity += -Physics2D.gravity.normalized * jumpMagnitude;
            }
            else
            {
                // set gravity to correspond to one of the touching grounds (for smooth inside-corner behavior)
                Physics2D.gravity = (Vector2)touching[UnityEngine.Random.Range(0, touching.Count)];

                // rotate counter-clockwise
                if (rotate_counter)
                {
                    playerRB.velocity -= Vector2.Perpendicular(Physics2D.gravity).normalized * forceMagnitude;
                }
                
                // rotate clockwise
                if (rotate_clock)
                {
                    playerRB.velocity += Vector2.Perpendicular(Physics2D.gravity).normalized * forceMagnitude;
                }

                // max velocity
                if ((rotate_counter || rotate_clock) && playerRB.velocity.magnitude > maxVelocity && !victory)
                {
                    playerRB.velocity = playerRB.velocity.normalized * maxVelocity;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // player touches ground: insert ground into front of touching dict, enable player jump
        if (collision.gameObject.CompareTag("Ground"))
        {
            Vector2 groundGravity = -collision.contacts[0].normal * gravityMagnitude;
            touching.Insert(0, collision.gameObject.GetInstanceID(), groundGravity);
            jumping = false;
        }
        // player is rejected by locked gate: push player backwards
        else if (collision.gameObject.CompareTag("Gate"))
        {
            playerRB.velocity += collision.contacts[0].normal * gateRejectionStrength;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // player stays touching ground: update touching dict gravity based on collision normal
        // (Physics2d gravity is modified in FixedUpdate for smooth inner-corner behavior)
        if (collision.gameObject.CompareTag("Ground"))
        {
            Vector2 groundGravity = -collision.contacts[0].normal * gravityMagnitude;
            touching[(object)collision.gameObject.GetInstanceID()] = groundGravity;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // player leaves ground: update touching dict
        if (collision.gameObject.CompareTag("Ground"))
        {
            // remove ground from touching dict
            touching.Remove(collision.gameObject.GetInstanceID());
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // player touches deadly object: player respawn
        if (collider.gameObject.CompareTag("Deadly"))
        {
            PlayerRespawn();
        }
        // player touches gate: unlock gate
        // (if player can't unlock, gate will collide, not be trigger)
        else if (collider.gameObject.CompareTag("Gate"))
        {
            UnlockGate(collider);
        }
        // player touches key: collect key
        else if (collider.gameObject.CompareTag("Key"))
        {
            collectedKeys.Enqueue(collider.gameObject);
            PlayerKeysChangedEvent?.Invoke();
        }
        // player touches victory gate: Victory!
        else if (collider.gameObject.CompareTag("Victory") && !victory)
        {
            victory = true;
            // invoke player victory event
            PlayerVictoryEvent?.Invoke();
            // update and save farthest level if not last level of game
            if (MenuController.currentLevel != MenuController.lastLevel)
            {
                MenuController.farthestLevel = Math.Max(MenuController.farthestLevel, SceneManager.GetActiveScene().buildIndex + 1);
                PlayerPrefs.SetInt("farthestLevel", MenuController.farthestLevel); // playerprefs saving ftw
                PlayerPrefs.Save();
            }
            StartCoroutine(PlayerVictoryAnimation());
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        // player goes out of bounds: player respawn
        if (collider.gameObject.CompareTag("Bounds"))
        {
            PlayerRespawn();
        }
    }

    // resets ordered dictionary of ground player is touching
    public void ResetTouching()
    {
        touching = new OrderedDictionary();
    }

    // unlock gate when player passes through with enough keys collected
    private void UnlockGate(Collider2D collider)
    {
        // dequeue the needed keys from collected and flag as used
        int gateKeys = collider.gameObject.GetComponent<GateController>().gateKeys;
        for (int i = 0; i < gateKeys; i++)
        {
            GameObject usedKey = collectedKeys.Dequeue();
            usedKey.GetComponent<KeyController>().keyUsed = true;
        }
        PlayerKeysChangedEvent?.Invoke();
        
        // update spawn location to be at gate
        Vector3 gatePosition = collider.transform.position;
        spawnPosition = new Vector3(gatePosition.x, gatePosition.y, 0);

        // update spawn gravity to be perpendicular to gate
        Vector3 gateParallelVector = collider.gameObject.GetComponent<Transform>().right;
        spawnGravity = new Vector2(gateParallelVector.x, gateParallelVector.y).normalized * -gravityMagnitude;
    }

    // update player key particle systemm graphics
    private void UpdatePlayerKeyPS()
    {
        // update key ps size
        float playerKeyScale = (playerKeySize + ((collectedKeys.Count - 1) * playerKeySizeIncrement));
        playerKeyPS.transform.localScale = new Vector3(playerKeyScale, playerKeyScale, playerKeyScale);

        // turn key ps on or off
        if (collectedKeys.Count == 0)
        {
            playerKeyPS.Stop();
        }  
        else
        {
            playerKeyPS.Play();
        }
    }

    // spawn player: reset physics values, reset player collections, play spawn animation
    private void PlayerSpawn()
    {
        // reset gravity
        Physics2D.gravity = spawnGravity;
        // reset player velocity
        playerRB.velocity = Vector3.zero;
        playerRB.angularVelocity = 0;
        // reset player position
        transform.position = spawnPosition;
        //reset player rotation
        transform.rotation = Quaternion.identity;

        // reset player touching dict and collected keys queue
        ResetTouching();
        collectedKeys = new Queue<GameObject>();
        PlayerKeysChangedEvent?.Invoke();

        // start spawn animation
        StartCoroutine(PlayerSpawnAnimation());
    }

    // handle animation for player spawn
    private IEnumerator PlayerSpawnAnimation()
    {
        // instantiate spawn ps, wait, destroy ps
        GameObject spawnAnimationObject = Instantiate(spawnPS, transform.position, transform.rotation);
        yield return new WaitForSeconds(spawnDuration);
        Destroy(spawnAnimationObject);
    }

    // respawn player: make player invisible unmovable and uninteractable, stop player motion,
    // play death animation, spawn player
    public void PlayerRespawn()
    {
        // only respawn if player not currently respawning or reveling in their Victory!
        if (!respawning && !victory)
        {
            // mark currently respawning
            respawning = true;

            // invoke player respawn event
            PlayerRespawnEvent?.Invoke();

            // invisible
            playerSR.enabled = false;
            playerKeyPS.Stop();
            // uninteractable
            playerCC.enabled = false;
            // unmovable
            playerRB.isKinematic = true;
            // stop motion
            playerRB.velocity = Vector3.zero;

            // start death animation, spawn player
            StartCoroutine(PlayerDeathAnimation());
        }
    }

    // handle animation for player death, spawn player
    private IEnumerator PlayerDeathAnimation()
    {
        // instantiate death ps, wait, destroy ps
        GameObject deathAnimationObject = Instantiate(deathPS, transform.position, transform.rotation);
        yield return new WaitForSeconds(respawnDuration);
        Destroy(deathAnimationObject);

        // make player visible, movable, interactable
        playerSR.enabled = true;
        playerRB.isKinematic = false;
        playerCC.enabled = true;

        // spawn player
        PlayerSpawn();

        // mark not currently respawning
        respawning = false;
    }

    // handle animation for player Victory!
    private IEnumerator PlayerVictoryAnimation()
    {
        // send player to Victory! velocity
        playerRB.velocity += playerRB.velocity.normalized * 2000;

        // set player key ps to Victory! size and play
        float playerKeyScale = (playerKeySize + (7 * playerKeySizeIncrement));
        playerKeyPS.transform.localScale = new Vector3(playerKeyScale, playerKeyScale, playerKeyScale);
        playerKeyPS.Play();

        // wait for phase 1 of animation
        yield return new WaitForSeconds(playerVictoryDuration - 1);

        // set player to reduced Victory! velocity and make player invisible
        playerRB.velocity = playerRB.velocity.normalized * 50;
        playerSR.enabled = false;

        // spawn a new player death ps every 0.1s for 1s
        for (int i = 0; i < 10; i++)
        {
            Instantiate(deathPS, transform.position, transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }

        // load Victory! menu
        SceneManager.LoadScene("Victory");
    }
}