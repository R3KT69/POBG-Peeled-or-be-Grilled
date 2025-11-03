using UnityEngine;
using TMPro;
using PurrNet;

public class PlayerMovementNet : NetworkIdentity
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpHeight = 1.8f;
    public float gravity = -30f;
    public float mouseSensi = 1f;
    public float rotationSpeed = 8f;
    private float yaw;
    public Animator animator; // assign your Animator here
    public Transform cameraTransform;

    private TMP_InputField inputField;
    private CharacterController characterController;

    [Header("Velocity Vector")]
    public Vector3 velocityVector;
    private bool isGrounded;

    
    [Header("Ground Check Settings")]
    public Transform feetTransformR;
    public Transform feetTransformL;
    public float sphereRadius = 0.3f;
    public LayerMask groundMask;
    private readonly Collider[] groundHits = new Collider[4];
    public float normalizedVelocity { get; private set; }
    private bool cursorLocked = true;


    void Start()
    {
        if (!isOwner) return;
        inputField = GameObject.Find("Input")?.GetComponent<TMP_InputField>();
        characterController = GetComponent<CharacterController>();
        cameraTransform = gameObject.GetComponentInChildren<Camera>().transform;
        animator = GetComponentInChildren<Animator>();
        yaw = transform.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!isOwner) return;

        //HandleMouseRotation();
        
        //if (inputField != null && inputField.isFocused) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            cursorLocked = !cursorLocked;
            Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !cursorLocked;
        }

        
        float moveHorizontal, moveVertical;

        GroundCheck();
        Gravity();
        RotatePlayerTowardCamera();
        Movement(out moveHorizontal, out moveVertical);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) velocityVector.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Mouse rotation
        

        // Animation Part
        float inputMagnitude = new Vector2(moveHorizontal, moveVertical).magnitude;
        normalizedVelocity = Mathf.Clamp01(inputMagnitude); // 0 = idle, 1 = full input
        if (animator != null) animator.SetFloat("Velocity", normalizedVelocity);

        float targetWeight = isGrounded ? 0f : 1f;
        float currentWeight = animator.GetLayerWeight(2);
        float newWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * 5);
        animator.SetLayerWeight(2, newWeight);
    }

    bool GroundCheckDriver(Transform feetTransformR, Transform feetTransformL, float sphereRadius, LayerMask groundMask)
    {
        int hitCountR = Physics.OverlapSphereNonAlloc(feetTransformR.position, sphereRadius, groundHits, groundMask);
        bool rightGrounded = hitCountR > 0;

        int hitCountL = Physics.OverlapSphereNonAlloc(feetTransformL.position, sphereRadius, groundHits, groundMask);
        bool leftGrounded = hitCountL > 0;

        return rightGrounded || leftGrounded;
    }

    private void GroundCheck()
    {
        isGrounded = GroundCheckDriver(feetTransformR, feetTransformL, sphereRadius, groundMask);
        if (isGrounded && velocityVector.y < 0) velocityVector.y = -2f;
    }

    private void Movement(out float moveHorizontal, out float moveVertical)
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveHorizontal + transform.forward * moveVertical;

        if (move.magnitude > 1f) move.Normalize(); // For fixing the increased diagonal movement bug

        characterController.Move(move * speed * Time.deltaTime);
    }

    private void Gravity()
    {
        velocityVector.y += gravity * Time.deltaTime;
        CollisionFlags flags = characterController.Move(velocityVector * Time.deltaTime);

        if ((flags & CollisionFlags.Above) != 0) // Stop upward movement when hit with ceiling
        {
            velocityVector.y = 0f;
            Vector3 pos = characterController.transform.position;
            pos.y -= 0.1f;
            characterController.transform.position = pos;

            velocityVector.y += gravity * Time.deltaTime;
        }
    }

    void OnDrawGizmos()
    {
        bool grounded = GroundCheckDriver(feetTransformR, feetTransformL, sphereRadius, groundMask);

        if (feetTransformR == null) return;
        Gizmos.color = grounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(feetTransformR.position, sphereRadius);

        if (feetTransformL == null) return;
        Gizmos.color = grounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(feetTransformL.position, sphereRadius);
    }

    private void HandleMouseRotation()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensi;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    public void RotatePlayerTowardCamera()
    {
        Vector3 inputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (inputDir.sqrMagnitude > 0.01f)
        {
            // Camera-relative direction
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;
            camForward.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    public void RotatePlayerTowardCameraForced()
    {
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(camForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}