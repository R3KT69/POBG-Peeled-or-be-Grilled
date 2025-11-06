using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    /// <summary>
    /// Camera script from ThirdPersonProject -r3kt
    /// </summary>
    
    [Header("Orbit Settings")]
    public float distance = 5f;
    public float mouseSensitivity = 2f;
    public float minYAngle = -30f;
    public float maxYAngle = 75f;

    [Header("Camera Offsets")]
    public Vector3 baseOffset = new Vector3(0f, 1.5f, 0f); // Camera position
    public Vector3 orbitOffset = new Vector3(0f, 0f, 0f);  // Free-look offset
    public Vector3 aimingOffset = new Vector3(0.5f, 0f, 1f);   // Aiming offset
    private bool isOffsetFlipped = false;

    [Header("Transition Settings")]
    public float bodyRotationSpeed = 10f;
    public float aimingShoulderSnapSpeed = 50f;
    public float offsetTransitionSpeed = 5f;

    [Header("Collision Settings")]
    public LayerMask collisionMask;
    public float cameraRadius = 0.3f;
    public float collisionOffset = 0.2f; // distance in front of wall

    private float xAngle = 0f;
    private float yAngle = 15f;

    private Transform playerTransform;
    private Vector3 currentShoulderOffset = Vector3.zero;
    private float aimBlend = 0f;
    private bool isAiming_ = false;

    void Start()
    {
        playerTransform = transform.parent;

        if (playerTransform == null)
        {
            enabled = false;
            return;
        }

        // UNRELIABLE CODE, REDACTED
        // MONOBEHAVIOUR (LOCAL) LOCKING FOR CURSOR, NO NEED TO LOCK CURSOR ON OTHER SCRIPTS. OTHERWISE EVERY CLIENT GETS LOCKED***
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void Update()
    {
        if (!playerTransform) return;

        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xAngle += mouseX;
        yAngle -= mouseY;
        yAngle = Mathf.Clamp(yAngle, minYAngle, maxYAngle);

        // Shoulder Snapping
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            isOffsetFlipped = !isOffsetFlipped;
            aimingOffset = new Vector3(
                isOffsetFlipped ? -Mathf.Abs(aimingOffset.x) : Mathf.Abs(aimingOffset.x),
                aimingOffset.y,
                aimingOffset.z
            );
        }

        Quaternion camRotation = Quaternion.Euler(yAngle, xAngle, 0f);

        if (Input.GetMouseButtonDown(1))
        {
            isAiming_ = !isAiming_;
        }

        bool isAiming = isAiming_;

        // Aim transition
        float targetBlend = isAiming ? 1f : 0f;
        aimBlend = Mathf.MoveTowards(aimBlend, targetBlend, Time.deltaTime * offsetTransitionSpeed);

        // Compute world-space offsets
        Vector3 orbitWorldOffset = orbitOffset;
        Vector3 aimingWorldOffset = camRotation * aimingOffset;
        Vector3 targetOffset = Vector3.Lerp(orbitWorldOffset, aimingWorldOffset, aimBlend);

        currentShoulderOffset = Vector3.Lerp(currentShoulderOffset, targetOffset, Time.deltaTime * aimingShoulderSnapSpeed);

        // Rotate player while aiming
        if (isAiming)
        {
            playerTransform.GetComponent<PlayerMovementNet>().RotatePlayerTowardCameraForced();

            // These dont work for some reason, used to work in the old project
            //Quaternion targetRotation = Quaternion.Euler(0f, xAngle, 0f); 
            //playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, Time.deltaTime * bodyRotationSpeed);
        }

        // Base camera position behind player
        Vector3 desiredCameraPos = playerTransform.position + baseOffset + currentShoulderOffset - camRotation * Vector3.forward * distance;

        // Wall collision prevention
        Vector3 collisionOrigin = playerTransform.position + baseOffset;
        Vector3 desiredDir = (desiredCameraPos - collisionOrigin).normalized;
        float desiredDistance = (desiredCameraPos - collisionOrigin).magnitude;

        if (Physics.SphereCast(collisionOrigin, cameraRadius, desiredDir, out RaycastHit hit, desiredDistance, collisionMask))
        {
            float hitDistance = Mathf.Max(hit.distance - collisionOffset, 0.1f);
            desiredCameraPos = collisionOrigin + desiredDir * hitDistance;
        }

        // Finally, apply shoulder offset after collision adjustment
        desiredCameraPos += currentShoulderOffset;


       
        transform.position = desiredCameraPos;
        transform.rotation = camRotation;
    }
}
