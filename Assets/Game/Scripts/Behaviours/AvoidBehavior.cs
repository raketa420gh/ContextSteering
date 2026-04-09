using UnityEngine;

public class AvoidBehavior : MonoBehaviour, IDangerBehavior
{
    [SerializeField]
    private float _detectionRadius = 10f;

    [SerializeField]
    private float _falloffSlots = 3f;

    [SerializeField]
    private float _maxIntensity = 1f;

    [SerializeField]
    private float _safetyMargin = 0.5f;

    private ContextSteeringAgent _agent;

    public void EvaluateDanger(ContextMap dangerMap, Transform agentTransform)
    {
        ObstacleComponent[] obstacles = FindObjectsOfType<ObstacleComponent>();

        if (obstacles == null || obstacles.Length == 0)
            return;

        if (_agent == null)
            _agent = agentTransform.GetComponent<ContextSteeringAgent>();

        float agentRadius = _agent != null ? _agent.Radius : 0f;
        Vector2 agentPos = new Vector2(agentTransform.position.x, agentTransform.position.z);

        foreach (ObstacleComponent obstacle in obstacles)
        {
            if (obstacle == null)
                continue;

            Vector2 obstaclePos = new Vector2(obstacle.transform.position.x, obstacle.transform.position.z);
            Vector2 toObstacle = obstaclePos - agentPos;
            float distance = toObstacle.magnitude;

            float threatRadius = obstacle.Radius + agentRadius + _safetyMargin;

            if (distance > _detectionRadius || distance < 0.01f)
                continue;

            float intensity;
            if (distance <= threatRadius)
            {
                intensity = _maxIntensity;
            }
            else
            {
                float dangerZone = _detectionRadius - threatRadius;
                float t = 1f - Mathf.Clamp01((distance - threatRadius) / Mathf.Max(dangerZone, 0.01f));
                intensity = Mathf.SmoothStep(0f, _maxIntensity, t);
            }

            float proximityFactor = Mathf.Clamp01(threatRadius / Mathf.Max(distance, 0.01f));
            float effectiveFalloff = _falloffSlots * (1f + proximityFactor * 2f);

            dangerMap.WriteValue(toObstacle.normalized, intensity, effectiveFalloff);
        }
    }
}