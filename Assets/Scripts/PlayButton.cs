using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    // References to the Button component and the SpawnManager
    private Button button;
    private SpawnManager spawnManager;

    // Reference to the AudioSource for playing sound effects
    private AudioSource audioSource;

    // Public variable for the button click sound effect, assignable in the Inspector
    public AudioClip buttonClickSound;

    void Start()
    {
        // Get the Button component attached to this GameObject
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Button component not found on this GameObject!");
            return;
        }

        // Find the SpawnManager in the scene and get its component
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if (spawnManager == null)
        {
            Debug.LogError("SpawnManager not found in the scene!");
            return;
        }

        // Get the AudioSource component attached to this GameObject
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on this GameObject!");
        }

        // Attach the OnPlayButtonClick method to the button's click event
        button.onClick.AddListener(OnPlayButtonClick);
    }

    // Method called when the play button is clicked
    void OnPlayButtonClick()
    {
        Debug.Log("Play button clicked.");

        // Play the button click sound if the AudioSource and sound clip are assigned
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
            Debug.Log("Button click sound played.");
        }
        else
        {
            Debug.LogWarning("Button click sound or AudioSource is not assigned.");
        }

        // Start the game by calling the StartGame method on the SpawnManager
        if (spawnManager != null)
        {
            spawnManager.StartGame();
            Debug.Log("Game started.");
        }
        else
        {
            Debug.LogError("SpawnManager is null!");
        }

        // Hide the play button after it is clicked
        gameObject.SetActive(false);
        Debug.Log("Play button hidden.");
    }
}
