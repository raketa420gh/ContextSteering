using UnityEngine;

public class MoveComponent : MonoBehaviour
{
    [SerializeField] 
    private float _moveSpeed = 5f;
    [SerializeField] 
    private float _rotationSmoothing = 10f;
    
    public void Move(Vector2 direction2D, float speedMultiplier)
    {
        if (direction2D.sqrMagnitude < 0.0001f)
            return;

        Vector3 moveDir = new Vector3(direction2D.x, 0f, direction2D.y).normalized;
        float finalSpeed = _moveSpeed * Mathf.Clamp01(speedMultiplier);

        transform.position += moveDir * finalSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * _rotationSmoothing
        );
    }
}