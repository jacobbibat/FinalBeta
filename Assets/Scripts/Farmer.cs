using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : MonoBehaviour
{
    // Static speed variable for all Farmer enemies, can be adjusted 
    public static float speed = 1.2f;

    // Private references for the Rigidbody component and the player GameObject
    private Rigidbody enemyRb;
    private GameObject player;

    // Prefabs for experience points and rare item drops
    public GameObject experiencePoints;
    public GameObject rareItem;

    // Drop rate threshold for rare item (20% chance)
    private float rareDr = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody component attached to this enemy
        enemyRb = GetComponent<Rigidbody>();

        // Find the player GameObject in the scene
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            // Move directly towards the player's position at the defined speed
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            
            // Rotate the enemy to face the player
            transform.LookAt(player.transform.position);
        }
    }

    // Called when the enemy is destroyed
    void OnDestroy()
    {
        Debug.Log("Enemy Destroyed");

        // Randomly decide what to drop when the enemy is destroyed
        float randomValue = Random.value; // Generates a value between 0.0 and 1.0

        if (randomValue >= rareDr)  // 80% chance to drop experience points
        {
            Instantiate(experiencePoints, transform.position, transform.rotation);
        }
        else  // 20% chance to drop a rare item (power-up)
        {
            Instantiate(rareItem, transform.position, transform.rotation);
        }
    }
}
