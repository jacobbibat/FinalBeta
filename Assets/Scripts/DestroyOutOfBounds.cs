using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    // Reference to the Main Camera's Transform component
    private Transform mainCameraTransform;

    // The maximum allowed distance from the camera before the object is destroyed
    public float maxDistanceFromCamera = 45f; // Set the distance limit 

    void Start()
    {
        // Cache the Main Camera's transform for performance efficiency
        mainCameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Calculate the distance between this object's position and the Main Camera's position
        float distanceFromCamera = Vector3.Distance(transform.position, mainCameraTransform.position);

        // If the object is further away than the allowed maximum distance, destroy it
        if (distanceFromCamera > maxDistanceFromCamera)
        {
            Destroy(gameObject); // Remove the object to free up resources
        }
    }
}
