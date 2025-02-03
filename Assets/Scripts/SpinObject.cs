using UnityEngine;

public class SpinObject : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Speed of rotation in degrees per second")]
    public float rotationSpeed = 180f;

    [Tooltip("Axis of rotation")]
    public Vector3 rotationAxis = Vector3.up;

    [Header("Optional Settings")]
    [Tooltip("Whether to randomize the initial rotation")]
    public bool randomizeInitialRotation = false;

    [Tooltip("Whether to reverse rotation direction")]
    public bool reverseRotation = false;

    private void Start()
    {
        // Normalize the rotation axis
        rotationAxis.Normalize();

        // Randomize initial rotation if enabled
        if (randomizeInitialRotation)
        {
            transform.rotation = Quaternion.Euler(
                Random.Range(0f, 360f),
                Random.Range(0f, 360f),
                Random.Range(0f, 360f)
            );
        }
    }

    private void Update()
    {
        // Calculate rotation amount this frame
        float rotationAmount = rotationSpeed * Time.deltaTime;
        
        // Apply reverse rotation if enabled
        if (reverseRotation)
        {
            rotationAmount *= -1;
        }

        // Rotate the object
        transform.Rotate(rotationAxis, rotationAmount);
    }
}