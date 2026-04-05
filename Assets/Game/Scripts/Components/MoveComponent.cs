using UnityEngine;

public class MoveComponent : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 5f;

    [SerializeField]
    private float _rotationSmoothing = 10f;

    [SerializeField]
    private float _accelerationTime = 0.5f;

    [SerializeField]
    private float _decelerationTime = 0.3f;

    private Vector3 _currentVelocity;
    private float _currentSpeed;

    public void Move(Vector2 direction2D, float speedMultiplier)
    {
        if (direction2D.sqrMagnitude < 0.0001f)
        {
            DecelerateToStop();
            return;
        }

        Vector3 moveDir = new Vector3(direction2D.x, 0f, direction2D.y).normalized;
        float targetSpeed = _moveSpeed * Mathf.Clamp01(speedMultiplier);

        AccelerateToSpeed(targetSpeed);
        ApplyMovement(moveDir);
        RotateTowards(moveDir);
    }

    private void AccelerateToSpeed(float targetSpeed)
    {
        float acceleration = _moveSpeed / Mathf.Max(_accelerationTime, 0.01f);
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, acceleration * Time.deltaTime);
    }

    private void DecelerateToStop()
    {
        float deceleration = _moveSpeed / Mathf.Max(_decelerationTime, 0.01f);
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, deceleration * Time.deltaTime);

        if (_currentSpeed > 0.01f && _currentVelocity.sqrMagnitude > 0.0001f)
        {
            ApplyMovement(_currentVelocity.normalized);
        }
    }

    private void ApplyMovement(Vector3 direction)
    {
        _currentVelocity = direction * _currentSpeed;
        transform.position += _currentVelocity * Time.deltaTime;
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * _rotationSmoothing
        );
    }
}