using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;            // Drag your player here
    public Vector3 offset = new Vector3(0, 2, 0); // Height offset

    [Header("Camera Settings")]
    public float distance = 5f;         // Default distance from player
    public float smoothSpeed = 10f;     // Camera smoothing for position
    public float cameraRadius = 0.3f;   // SphereCast radius
    public LayerMask collisionMask;     // Set to everything except player layer

    [Header("Rotation Settings")]
    public float verticalRotationSpeed = 5f;   // Mouse vertical sensitivity
    public float minYAngle = -30f;
    public float maxYAngle = 60f;
    public float rotationSmoothness = 10f;

    private float pitch; // X rotation
    private Vector3 currentRotation;

    void Start()
    {
        currentRotation = transform.eulerAngles;
        pitch = currentRotation.x;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Read vertical mouse input for camera pitch
        pitch -= Input.GetAxis("Mouse Y") * verticalRotationSpeed;
        pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);

        // Horizontal rotation follows player
        float yaw = target.eulerAngles.y;

        // Smooth rotation
        Vector3 targetRotation = new Vector3(pitch, yaw, 0);
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * rotationSmoothness);
        Quaternion rotationQuat = Quaternion.Euler(currentRotation);

        // Desired camera position behind the player
        Vector3 desiredPosition = target.position + offset - rotationQuat * Vector3.forward * distance;

        // Check for walls
        RaycastHit hit;
        Vector3 direction = (desiredPosition - (target.position + offset)).normalized;
        float targetDistance = distance;

        if (Physics.SphereCast(target.position + offset, cameraRadius, direction, out hit, distance, collisionMask))
        {
            targetDistance = hit.distance - cameraRadius;
            desiredPosition = (target.position + offset) + direction * targetDistance;
        }

        // Smoothly move camera
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

        // Apply rotation
        transform.rotation = rotationQuat;
    }
}
