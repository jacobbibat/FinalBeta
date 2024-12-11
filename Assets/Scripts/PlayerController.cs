using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Power-up fields
    public bool hasPowerup;                       
    public GameObject powerupIndicator;          
    public int powerUpDuration = 8;               
    public AudioClip projectileSound;             
    public AudioClip powerupSound;                
    public AudioClip collisionSound;
    public AudioClip scoreSound;
    private AudioSource playerAudio;              

    // Projectile (Egg) fields
    public GameObject eggPrefab;                  
    private float eggCd = 0.6f;                   // Default cooldown between shots
    private float eggDefaultCd = 0.6f;            // Stores the default cooldown for reset
    private float eggSpawn = 0.0f;                // Timer to track cooldown progress

    // Player movement fields
    public static float speed = 5.0f;                   
    public float rotationSpeed = 10.0f;           // Controls the smoothness of rotation
    private Vector3 shootDirection = Vector3.forward; // Default shooting directio

    // Reference to the SpawnManager
    private SpawnManager spawnManager;            // To manage game states like game over and scoring

    // Game Over variables
    private int maxHits = 5;                      // Max hits the player can take before game over
    private int currentHits = 0;                  

    // Game Over UI
    public GameObject gameOverText;               // Reference to the Game Over UI text

    void Start()
    {
        // Locate and reference the SpawnManager in the scene
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        // Get the AudioSource component attached to the player for sound effects
        playerAudio = GetComponent<AudioSource>();

        // Hide the Game Over text initially
        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }
    }

    void Update()
    {
        // Only allow movement and shooting if the game is active
        if (spawnManager.isGameActive)
        {
            HandleMovement();          // Handle player movement and rotation
            HandleProjectile();        // Handle shooting projectiles
            UpdatePowerupIndicator();  
        }
    }

    // Handles player movement and rotation
    private void HandleMovement()
    {
        // Capture player input for horizontal (X) and vertical (Z) movement
        float speedX = Input.GetAxis("Horizontal");
        float speedZ = Input.GetAxis("Vertical");

        if (speedX != 0 || speedZ != 0)
        {
            // Calculate movement direction and normalize it
            shootDirection = new Vector3(speedX, 0, speedZ).normalized;

            // Smoothly rotate the player to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(shootDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // Move the player based on input and speed
        Vector3 movement = new Vector3(speedX, 0, speedZ) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }

    // Handles shooting projectiles (eggs)
    private void HandleProjectile()
    {
        // Increment the cooldown timer
        eggSpawn += Time.deltaTime;

        // Spawn projectile if the cooldown has passed
        if (eggSpawn >= eggCd)
        {
            Instantiate(eggPrefab, transform.position, Quaternion.LookRotation(shootDirection));
            eggSpawn = 0.0f; // Reset cooldown timer

            // Play the projectile shooting sound
            playerAudio.PlayOneShot(projectileSound, 0.5f);
        }
    }

    // Updates the position of the power-up indicator
    private void UpdatePowerupIndicator()
    {
        // Keep the power-up indicator floating slightly above the player
        powerupIndicator.transform.position = transform.position + new Vector3(0, 1f, 0);
    }

    // Handles collisions with different game objects
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy")) // Collision with an enemy
        {
            // Play the collision sound
            if (collisionSound != null)
            {
                playerAudio.PlayOneShot(collisionSound, 1.0f);
            }

            Debug.Log("Player hit by an enemy!");

            // Increase the hit count
            currentHits++;

            // If the player takes too many hits, end the game
            if (currentHits >= maxHits)
            {
                GameOver();
            }

            // Destroy the enemy upon collision
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Experience")) // Collect experience points
        {
            Destroy(other.gameObject);
            spawnManager.UpdateScore(5); // Increase the score by 5 points
            playerAudio.PlayOneShot(scoreSound, 0.7f);
        }
        else if (other.gameObject.CompareTag("Powerup")) // Collect a power-up
        {
            Debug.Log("Powerup ACTIVATED");
            ActivatePowerup(other);
        }
    }

    // Activates the power-up and modifies shooting behavior
    private void ActivatePowerup(Collider powerup)
    {
        hasPowerup = true;                   // Set the power-up flag to true
        powerupIndicator.SetActive(true);    // Show the power-up indicator
        Destroy(powerup.gameObject);         

        // Play the power-up activation sound
        playerAudio.PlayOneShot(powerupSound, 1.0f);

        // Increase firing rate by reducing the egg cooldown
        eggCd = eggDefaultCd / 10;

        // Start the power-up cooldown timer
        StartCoroutine(PowerupCooldown());
    }

    // Coroutine to handle the power-up duration
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);

        // Reset power-up effects after the duration ends
        hasPowerup = false;
        eggCd = eggDefaultCd;          // Reset the egg cooldown to default
        powerupIndicator.SetActive(false); 
    }

    // Handles the game over scenario
    private void GameOver()
    {
        // Stop spawning enemies by calling GameOver on SpawnManager
        spawnManager.GameOver();

        // Display the Game Over UI text
        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
        }

        // Disable the player GameObject to stop movement and shooting
        this.gameObject.SetActive(false);

        // Log game over message
        Debug.Log("Game Over!");
    }
}
