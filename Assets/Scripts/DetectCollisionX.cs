using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisionsX : MonoBehaviour
{
    private SpawnManager spawnManager;
    private AudioSource audioSource;

    public AudioClip collisionSound;

    void Start()
    {
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        audioSource = GetComponent<AudioSource>(); // Ensure the object has an AudioSource component
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy")) // Check if the object that collided is an enemy
        {
            audioSource.PlayOneShot(collisionSound, 1.0f);

            // Destroy the enemy
            Destroy(other.gameObject);
            spawnManager.UpdateScore(7);

            // Destroy the player object (optional: for when the player is hit)
            Destroy(gameObject);
        }
    }
}
