using UnityEngine;

public class ChaseBehavior : MonoBehaviour, IInterestContextBehavior
{
    [SerializeField] 
    private string _targetTag = "Target";
    [SerializeField] 
    private float _maxDistance = 20f;
    [SerializeField] 
    private float _falloffSlots = 3f;
    
    [SerializeField] 
    private float _maxIntensity = 1f;

    public void EvaluateInterest(ContextMap interestMap, Transform agentTransform)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(_targetTag);

        Vector2 agentPos = ToVec2(agentTransform.position);

        foreach (GameObject target in targets)
        {
            Vector2 targetPos = ToVec2(target.transform.position);
            Vector2 toTarget = targetPos - agentPos;
            float distance = toTarget.magnitude;

            if (distance > _maxDistance || distance < 0.01f)
                continue;
            
            float normalizedDistance = 1f - Mathf.Clamp01(distance / _maxDistance);
            float intensity = normalizedDistance * _maxIntensity;

            Vector2 direction = toTarget.normalized;
            interestMap.WriteValue(direction, intensity, _falloffSlots);
        }
    }

    public void EvaluateDanger(ContextMap dangerMap, Transform agentTransform)
    {
        
    }

    private Vector2 ToVec2(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
}