using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrain = 25f;
    public float staminaRegen = 20f;

    [Header("Crouch")]
    public float standingHeight = 2f;
    public float crouchingHeight = 1.1f;
    public float crouchCameraOffset = 0.45f;
    public float crouchSpeedTransition = 12f;

    [Header("Mouse")]
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    [Header("UI")]
    public Slider staminaBar;

    private CharacterController controller;
    private float verticalRotation;
    private float verticalVelocity;
    private float stamina;

    private Vector3 standingCameraPosition;
    private bool isCrouching;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        stamina = maxStamina;

        standingCameraPosition = cameraTransform.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (staminaBar != null)
            staminaBar.value = 1f;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleCrouch();
        HandleStamina();
    }

    void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 1f)
            move.Normalize();

        bool sprinting =
            Input.GetKey(KeyCode.LeftShift) &&
            z > 0.1f &&
            !isCrouching &&
            stamina > 0f;

        float speed = isCrouching
            ? crouchSpeed
            : sprinting
                ? sprintSpeed
                : walkSpeed;

        controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        if (Input.GetKeyDown(KeyCode.Space) &&
            controller.isGrounded &&
            !isCrouching)
        {
            verticalVelocity = Mathf.Sqrt(
                jumpHeight * -2f * gravity
            );
        }

        verticalVelocity += gravity * Time.deltaTime;

        controller.Move(
            Vector3.up * verticalVelocity * Time.deltaTime
        );
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;

        verticalRotation = Mathf.Clamp(
            verticalRotation,
            -90f,
            90f
        );

        cameraTransform.localRotation =
            Quaternion.Euler(
                verticalRotation,
                0f,
                0f
            );
    }

    void HandleCrouch()
    {
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        float targetHeight =
            isCrouching
                ? crouchingHeight
                : standingHeight;

        controller.height = Mathf.Lerp(
            controller.height,
            targetHeight,
            crouchSpeedTransition * Time.deltaTime
        );

        controller.center = new Vector3(
            0f,
            controller.height / 2f,
            0f
        );

        float targetCameraY =
            standingCameraPosition.y -
            (isCrouching ? crouchCameraOffset : 0f);

        Vector3 targetCameraPosition = new Vector3(
            standingCameraPosition.x,
            targetCameraY,
            standingCameraPosition.z
        );

        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition,
            targetCameraPosition,
            crouchSpeedTransition * Time.deltaTime
        );
    }

    void HandleStamina()
    {
        float z = Input.GetAxisRaw("Vertical");

        bool sprinting =
            Input.GetKey(KeyCode.LeftShift) &&
            z > 0.1f &&
            !isCrouching &&
            stamina > 0f;

        if (sprinting)
            stamina -= staminaDrain * Time.deltaTime;
        else
            stamina += staminaRegen * Time.deltaTime;

        stamina = Mathf.Clamp(
            stamina,
            0f,
            maxStamina
        );

        if (staminaBar != null)
            staminaBar.value = stamina / maxStamina;
    }
}
