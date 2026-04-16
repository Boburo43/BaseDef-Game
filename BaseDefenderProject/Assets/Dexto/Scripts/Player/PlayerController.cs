using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class TopDownCharacterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float acceleration = 20f;
    public float gravity = -20f;

    [Header("Rotation")]
    [Tooltip("Degrees per second. Set to 0 for instant snap.")]
    public float rotationSpeed = 720f;

    [Header("Animation")]
    public Animator animator;

    private CharacterController _cc;
    private Vector3 _velocity;
    private float _verticalSpeed;
    private Vector2 _moveInput;

    private static readonly int HashSpeed = Animator.StringToHash("Speed");

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    
    void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        if(GameModeManager.Instance.CurrentMode == GameMode.Move)
        {
            HandleMovement();
            ApplyGravity();
        }
    }

    void HandleMovement()
    {
        Vector3 inputDir = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;
        Vector3 targetVelocity = inputDir * moveSpeed;
        _velocity = Vector3.MoveTowards(_velocity, targetVelocity, acceleration * Time.deltaTime);

        if (inputDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(inputDir, Vector3.up);

            if (rotationSpeed <= 0f)
                transform.rotation = targetRot;
            else
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        if (animator != null)
            animator.SetFloat(HashSpeed, _velocity.magnitude / moveSpeed);
    }

    void ApplyGravity()
    {
        // Small negative value keeps CharacterController.isGrounded reliable on the next frame.
        if (_cc.isGrounded && _verticalSpeed < 0f)
            _verticalSpeed = -2f;
        else
            _verticalSpeed += gravity * Time.deltaTime;

        _cc.Move((_velocity + Vector3.up * _verticalSpeed) * Time.deltaTime);
    }
}