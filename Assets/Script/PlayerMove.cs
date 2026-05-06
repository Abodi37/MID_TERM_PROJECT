using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AdvancedPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 1.5f;
    public float gravity = -19.62f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float crouchTransitionSpeed = 10f;
    private float standingHeight;
    private Vector3 standingCenter;
    private bool isCrouching;
    private bool wantsToCrouch; 

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 15f;
    public float minStaminaToSprint = 25f;

    [Header("Camera & FOV Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float normalFOV = 60f;
    public float sprintFOV = 75f;
    public float fovTransitionSpeed = 8f;

    private float xRotation = 0f;
    private Camera camComponent;
    private Vector3 originalCameraPosition;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isSprinting;
    private bool isExhausted;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina;

        standingHeight = controller.height;
        standingCenter = controller.center;

        if (playerCamera != null)
        {
            camComponent = playerCamera.GetComponent<Camera>();
            if (camComponent != null) camComponent.fieldOfView = normalFOV;
            originalCameraPosition = playerCamera.localPosition;
        }
        else
        {
            
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleRotation();
        HandleCrouch();
        HandleAllMovement();
        HandleFOV();
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (playerCamera != null)
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleCrouch()
    {
        
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
        {
            wantsToCrouch = !wantsToCrouch;
        }

        
        if (!wantsToCrouch && isCrouching)
        {
            Vector3 rayStart = transform.position + Vector3.up * (controller.center.y + (standingHeight / 2f) + 0.1f);
            if (Physics.Raycast(rayStart, Vector3.up, 0.5f))
            {
                wantsToCrouch = true;
                
            }
        }

        isCrouching = wantsToCrouch;

        
        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        float targetCenterY = isCrouching ? standingCenter.y - ((standingHeight - crouchHeight) / 2f) : standingCenter.y;

        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        controller.center = Vector3.Lerp(controller.center, new Vector3(standingCenter.x, targetCenterY, standingCenter.z), Time.deltaTime * crouchTransitionSpeed);

        
        if (playerCamera != null)
        {
            float targetCamPosY = isCrouching ? originalCameraPosition.y - (standingHeight - crouchHeight) : originalCameraPosition.y;
            Vector3 targetCamPos = new Vector3(originalCameraPosition.x, targetCamPosY, originalCameraPosition.z);
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, targetCamPos, Time.deltaTime * crouchTransitionSpeed);
        }
    }

    private void HandleAllMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        if (move.magnitude > 1f) move.Normalize();

        bool isTryingToSprint = Input.GetKey(KeyCode.LeftShift) && move.magnitude > 0.1f && !isExhausted && !isCrouching;

        if (isTryingToSprint && currentStamina > 0)
        {
            isSprinting = true;
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isExhausted = true;
                isSprinting = false;
            }
        }
        else
        {
            isSprinting = false;
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina >= maxStamina) currentStamina = maxStamina;
            if (isExhausted && currentStamina >= minStaminaToSprint) isExhausted = false;
        }

        float currentSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
        Vector3 horizontalVelocity = move * currentSpeed;

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = horizontalVelocity + (Vector3.up * velocity.y);
        controller.Move(finalMove * Time.deltaTime);
    }

    private void HandleFOV()
    {
        if (camComponent != null)
        {
            float targetFOV = isSprinting ? sprintFOV : normalFOV;
            camComponent.fieldOfView = Mathf.Lerp(camComponent.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
        }
    }
}