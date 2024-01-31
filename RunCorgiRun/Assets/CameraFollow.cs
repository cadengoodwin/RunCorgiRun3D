using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;       // Reference to the player's transform
    public float smoothSpeed = 0.125f; // Smoothing factor
    public Vector3 offset;         // Offset from the player position

    private bool shouldFollow = false; // Flag to determine if the camera should follow

    // This method will start the camera follow functionality
    public void StartFollowing()
    {
        shouldFollow = true;
    }

    // If you ever need to stop the camera from following
    public void StopFollowing()
    {
        shouldFollow = false;
    }

    private void FixedUpdate()
    {
        if (shouldFollow)
        {
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}