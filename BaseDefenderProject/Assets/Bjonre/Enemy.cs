using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    private Vector3 targetPosition;
    private bool hasTarget = false;

    private void Update()
    {
        if (!hasTarget)
            return;

        MoveToTarget();
    }

    public void SetTargetPosition(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
        hasTarget = true;
    }

    private void MoveToTarget()
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance < 0.05f)
        {
            transform.position = targetPosition;
            hasTarget = false;
            return;
        }

        direction.Normalize();
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}