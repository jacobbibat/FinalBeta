using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    // Public references for GameObjects and UI elements
    public GameObject enemyPrefab;         // Prefab for the enemy to spawn
    public GameObject player;            
    public TextMeshProUGUI scoreText;      // UI text to display the score
    public int waveNumber = 1;             // Current wave number
    public bool isGameActive = false;      // Flag to check if the game is active
    public GameObject titleScreen;         // Title screen UI element
    public AudioClip nextWaveSound;        // Sound clip for starting the next wave
    public AudioSource audioSource;      

    // Game Over UI
    public GameObject gameOverText;        

    // Private variables for internal game state
    private int score = 0;                 // Player's scoren set to default 0
    private float spawnTimer = 0.0f;       
    private float spawnInterval = 0.15f;    // Interval between enemy spawns
    private int enemiesToSpawnPerWave = 35;// Total enemies to spawn per wave
    private int enemiesSpawnedThisWave = 0; // Counter for enemies spawned in the current wave
    private float spawnDistance = 30.0f;   // Distance from the player where enemies will spawn

    void Start()
    {
        isGameActive = false;  // The game starts in an inactive state

        // Hide the Game Over text initially if it exists
        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }

        // Get the AudioSource 
        audioSource = GetComponent<AudioSource>(); 
    }

    void Update()
    {
        // Exit early if the game is not active
        if (!isGameActive) return;

        // Track the spawn timer
        spawnTimer += Time.deltaTime;

        // Spawn an enemy when the timer exceeds the spawn interval and we haven't reached the spawn limit
        if (spawnTimer >= spawnInterval && enemiesSpawnedThisWave < enemiesToSpawnPerWave)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
            enemiesSpawnedThisWave++;
            spawnTimer = 0.0f;  // Reset the spawn timer
        }

        // Check if all enemies are killed plan for next wave of enemies to spawn
        if (Object.FindObjectsByType<Farmer>(FindObjectsSortMode.None).Length == 0 &&
            enemiesSpawnedThisWave == enemiesToSpawnPerWave)
        {
            waveNumber++;                  // Increment the wave number
            enemiesSpawnedThisWave = 0;    // Reset the enemy spawn counter
            SpawnEnemyWave(waveNumber);    // Start the next wave
            Debug.Log("NEXT WAVE");        
            audioSource.PlayOneShot(nextWaveSound, 1.5f);  // Play the next wave sound effect
        }
    }

    // Method to spawn a wave of enemies
    void SpawnEnemyWave(int enemiesToSpawn)
    {
        // Increase enemy speed each wave to make the game progressively harder
        Farmer.speed *= 1.3f;
        PlayerController.speed *= 1.125f;

        // Spawn the specified number of enemies
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }
    }

    // Method to generate a random spawn position around the player
    private Vector3 GenerateSpawnPosition()
    {
        Vector3 randomDirection = Random.onUnitSphere;  // Get a random direction on a sphere
        randomDirection.y = 0;  // Keep the spawn position on the same horizontal level
        return player.transform.position + randomDirection * spawnDistance;  // Offset from the player's position
    }

    // Method to update the player's score and refresh the score UI
    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;                        // Add the specified amount to the score
        scoreText.text = "Score: " + score;         // Update the score text display
    }

    // Method to start the game when the play button is pressed
    public void StartGame()
    {
        isGameActive = true;        // Activate the game
        score = 0;                  // Reset the score
        enemiesSpawnedThisWave = 0; // Reset enemy spawn count
        waveNumber = 1;             // Reset to the first wave
        UpdateScore(0);             // Refresh the score display

        // Start the first wave immediately
        SpawnEnemyWave(waveNumber);

        // Hide the title screen
        titleScreen.gameObject.SetActive(false);
    }

    // Method to stop the game and display the Game Over screen
    public void GameOver()
    {
        isGameActive = false;  // Deactivate the game

        if (gameOverText != null)
        {
            gameOverText.SetActive(true);  // Display the Game Over message
        }
    }
}
