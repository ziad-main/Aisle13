using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeedMultiplier = 1.5f;
    public float gravity = -9.81f;
    
    [Header("Camera Shake Settings")]
    public Transform cameraTransform;
    public float walkShakeIntensity = 0.02f;
    public float sprintShakeIntensity = 0.05f;
    public float shakeSpeed = 10f;
    
    [Header("Flashlight Settings")]
    public Light flashlight;
    public KeyCode flashlightKey = KeyCode.F;
    
    [HideInInspector] public Vector3 moveDirection;
    
    private float hInput, vInput;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isSprinting;
    private Vector3 originalCameraPosition;
    private float shakeTimer;
    private bool isFlashlightOn;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (cameraTransform != null)
        {
            originalCameraPosition = cameraTransform.localPosition;
        }
        
        // Initialize flashlight state
        if (flashlight != null)
        {
            isFlashlightOn = flashlight.enabled;
        }
    }

    void Update()
    {
        GetDirectionAndMove();
        HandleCameraShake();
        HandleFlashlight();
    }
    
    void GetDirectionAndMove()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        
        // Check for sprint input
        isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        
        // Calculate movement
        moveDirection = transform.forward * vInput + transform.right * hInput;
        
        // Apply sprint multiplier if sprinting
        float currentSpeed = isSprinting ? moveSpeed * sprintSpeedMultiplier : moveSpeed;
        
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }
    
    void HandleCameraShake()
    {
        if (cameraTransform == null) return;
        
        // Only shake when moving
        bool isMoving = moveDirection.magnitude > 0.1f;
        
        if (isMoving)
        {
            shakeTimer += Time.deltaTime * shakeSpeed;
            
            // Choose shake intensity based on sprint state
            float currentShakeIntensity = isSprinting ? sprintShakeIntensity : walkShakeIntensity;
            
            // Generate shake using Perlin noise for smooth movement
            float shakeX = (Mathf.PerlinNoise(shakeTimer, 0f) - 0.5f) * currentShakeIntensity;
            float shakeY = (Mathf.PerlinNoise(0f, shakeTimer) - 0.5f) * currentShakeIntensity;
            
            // Apply shake to camera
            cameraTransform.localPosition = originalCameraPosition + new Vector3(shakeX, shakeY, 0f);
        }
        else
        {
            // Smoothly return to original position when not moving
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition, 
                originalCameraPosition, 
                Time.deltaTime * 10f
            );
        }
    }
    
    void HandleFlashlight()
    {
        if (flashlight == null) return;
        
        // Toggle flashlight on key press
        if (Input.GetKeyDown(flashlightKey))
        {
            isFlashlightOn = !isFlashlightOn;
            flashlight.enabled = isFlashlightOn;
        }
    }
}