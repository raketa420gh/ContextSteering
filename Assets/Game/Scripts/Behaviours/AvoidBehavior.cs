using UnityEngine;

public class AvoidBehavior : MonoBehaviour, IDangerBehavior
{
    [SerializeField] 
    private string _obstacleTag = "Obstacle";
    [SerializeField] 
    private float _minDistance = 2f;
    [SerializeField] 
    private float _detectionRadius = 8f;
    [SerializeField] 
    private float _falloffSlots = 2f;
    [SerializeField] 
    private float _maxIntensity = 1f;

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

            float intensity;
            if (distance <= _minDistance)
            {
                intensity = _maxIntensity;
            }
            else
            {
                float t = 1f - Mathf.Clamp01(
                    (distance - _minDistance) / (_detectionRadius - _minDistance)
                );
                intensity = t * _maxIntensity;
            }

            dangerMap.WriteValue(toObstacle.normalized, intensity, _falloffSlots);
        }
    }

    private Vector2 ToVec2(Vector3 v) => new Vector2(v.x, v.z);
}