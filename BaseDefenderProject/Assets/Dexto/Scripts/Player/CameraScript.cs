using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownCameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Positioning")]
    public float height = 15f;
    public float distanceOffset = 12f;
    public float smoothTime = 0.15f;

    [Header("Angle")]
    [Range(10f, 85f)]
    public float pitchAngle = 50f;

    [Header("Mouse Influence")]
    [Tooltip("How far the camera can move towards the mouse.")]
    public float mouseInfluenceDistance = 5f;

    [Tooltip("Smoothing for the mouse shift.")]
    public float mouseSmoothing = 0.5f;

    private Vector3 _posVelocity;
    private Vector3 _mouseOffset;
    private Vector3 _mouseRefVelocity;

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Get Mouse Position using New Input System
        Vector2 mousePosition = Pointer.current.position.ReadValue();

        // 2. Normalize Mouse Position (-1 to 1)
        // Center of screen is (0,0)
        float x = (mousePosition.x / Screen.width) - 0.5f;
        float y = (mousePosition.y / Screen.height) - 0.5f;

        // Multiply by 2 so that the edges of the screen represent the full influence distance
        Vector3 targetMouseOffset = new Vector3(x * 2f, 0, y * 2f) * mouseInfluenceDistance;

        // 3. Smooth the Mouse Offset
        _mouseOffset = Vector3.SmoothDamp(
            _mouseOffset,
            targetMouseOffset,
            ref _mouseRefVelocity,
            mouseSmoothing);

        // 4. Calculate Desired Position
        // Height (Y) and Distance Back (Z) relative to the player
        Vector3 verticalOffset = Vector3.up * height;
        Vector3 horizontalOffset = Vector3.back * distanceOffset;

        Vector3 desiredPos = target.position + verticalOffset + horizontalOffset + _mouseOffset;

        // 5. Apply Smooth Movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref _posVelocity,
            smoothTime);

        // 6. Apply Rotation
        transform.rotation = Quaternion.Euler(pitchAngle, 0f, 0f);
    }
}