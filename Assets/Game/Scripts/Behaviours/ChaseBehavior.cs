using UnityEngine;

public class ChaseBehavior : MonoBehaviour, IInterestBehavior
{
    [SerializeField] 
    private Transform _target;
    [SerializeField] 
    private float _maxDistance = 30f;
    [SerializeField] 
    private float _falloffSlots = 3f;
    [SerializeField] 
    private float _maxIntensity = 1f;
    [SerializeField] 
    private float _arrivalDistance = 0.5f;

    public void EvaluateInterest(ContextMap interestMap, Transform agentTransform)
    {
        if (_target == null)
            return;

        Vector2 agentPos = ToVec2(agentTransform.position);
        Vector2 targetPos = ToVec2(_target.position);
        Vector2 toTarget = targetPos - agentPos;
        float distance = toTarget.magnitude;

        if (distance < _arrivalDistance || distance > _maxDistance)
            return;

        float normalizedDistance = 1f - Mathf.Clamp01(distance / _maxDistance);
        float intensity = normalizedDistance * _maxIntensity;

        intensity = Mathf.Max(intensity, 0.1f);

        interestMap.WriteValue(toTarget.normalized, intensity, _falloffSlots);
    }

    private Vector2 ToVec2(Vector3 v) => new Vector2(v.x, v.z);
}