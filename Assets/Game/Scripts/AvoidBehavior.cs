using UnityEngine;

public class AvoidBehavior : MonoBehaviour, IContextBehavior
{
    [SerializeField] 
    private string _obstacleTag = "Obstacle";
    [SerializeField] 
    private float _minDistance = 3f;
    [SerializeField] 
    private float _detectionRadius = 8f;
    [SerializeField] 
    private float _falloffSlots = 2f;
    
    [SerializeField] 
    private float _maxIntensity = 1f;

    public void EvaluateInterest(ContextMap interestMap, Transform agentTransform)
    {
        
    }

    public void EvaluateDanger(ContextMap dangerMap, Transform agentTransform)
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag(_obstacleTag);

        Vector2 agentPos = ToVec2(agentTransform.position);

        foreach (GameObject obstacle in obstacles)
        {
            Vector2 obstaclePos = ToVec2(obstacle.transform.position);
            Vector2 toObstacle = obstaclePos - agentPos;
            float distance = toObstacle.magnitude;

            if (distance > _detectionRadius || distance < 0.01f)
                continue;
            
            float normalizedProximity = 1f - Mathf.Clamp01(
                (distance - _minDistance) / (_detectionRadius - _minDistance)
            );

            if (distance > _minDistance)
            {
                normalizedProximity *= 0.5f;
            }

            float intensity = normalizedProximity * _maxIntensity;

            Vector2 direction = toObstacle.normalized;
            dangerMap.WriteValue(direction, intensity, _falloffSlots);
        }
    }

    private Vector2 ToVec2(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
}