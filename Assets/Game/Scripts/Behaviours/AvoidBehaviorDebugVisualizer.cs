using UnityEngine;

[RequireComponent(typeof(AvoidBehavior))]
public class AvoidBehaviorDebugVisualizer : MonoBehaviour
{
    [Header("Gizmos Settings")]
    [SerializeField] 
    private bool _showGizmos = true;
    [SerializeField] 
    private bool _showConnections = true;
    [SerializeField] 
    private bool _showDetectionRadius = true;
    [SerializeField] 
    private bool _showMinDistanceRadius = true;
    [SerializeField] 
    private Color _detectionRadiusColor = new Color(1f, 0.3f, 0f, 0.12f);
    [SerializeField] 
    private Color _minDistanceColor = new Color(1f, 0f, 0f, 0.25f);
    [SerializeField] 
    private Color _dangerConnectionColor = new Color(1f, 0.2f, 0.1f, 0.7f);
    [SerializeField] 
    private Color _safeConnectionColor = new Color(1f, 1f, 1f, 0.15f);

    [Header("Synced from AvoidBehavior")]
    [SerializeField] 
    private string _obstacleTag = "Obstacle";
    [SerializeField] 
    private float _minDistance = 2f;
    [SerializeField] 
    private float _detectionRadius = 8f;

    private void OnValidate()
    {
        SyncFromBehavior();
    }

    private void Reset()
    {
        SyncFromBehavior();
    }

    private void SyncFromBehavior()
    {
        var behavior = GetComponent<AvoidBehavior>();
        if (behavior == null) return;

#if UNITY_EDITOR
        var so = new UnityEditor.SerializedObject(behavior);
        var tagProp = so.FindProperty("_obstacleTag");
        var minProp = so.FindProperty("_minDistance");
        var detProp = so.FindProperty("_detectionRadius");

        if (tagProp != null) _obstacleTag = tagProp.stringValue;
        if (minProp != null) _minDistance = minProp.floatValue;
        if (detProp != null) _detectionRadius = detProp.floatValue;
#endif
    }

    private void OnDrawGizmosSelected()
    {
        if (!_showGizmos) return;

        Vector3 agentPos = transform.position;
        
        if (_showDetectionRadius)
        {
            UnityEditor.Handles.color = _detectionRadiusColor;
            UnityEditor.Handles.DrawSolidDisc(agentPos, Vector3.up, _detectionRadius);

            Gizmos.color = new Color(
                _detectionRadiusColor.r,
                _detectionRadiusColor.g,
                _detectionRadiusColor.b,
                0.5f
            );
            DrawCircleXZ(agentPos, _detectionRadius, 64);
        }
        
        if (_showMinDistanceRadius)
        {
            UnityEditor.Handles.color = _minDistanceColor;
            UnityEditor.Handles.DrawSolidDisc(agentPos, Vector3.up, _minDistance);
            
            Gizmos.color = new Color(
                _minDistanceColor.r,
                _minDistanceColor.g,
                _minDistanceColor.b,
                0.8f
            );
            DrawCircleXZ(agentPos, _minDistance, 48);
        }

        if (!_showConnections) return;
        
        GameObject[] obstacles;
        try
        {
            obstacles = GameObject.FindGameObjectsWithTag(_obstacleTag);
        }
        catch
        {
            return;
        }

        Vector2 agentPos2D = new Vector2(agentPos.x, agentPos.z);

        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle == this.gameObject) continue;

            Vector3 obstaclePos = obstacle.transform.position;
            Vector2 obstaclePos2D = new Vector2(obstaclePos.x, obstaclePos.z);
            float distance = (obstaclePos2D - agentPos2D).magnitude;

            if (distance < 0.01f) continue;

            bool inRange = distance <= _detectionRadius;

            if (inRange)
            {
                float intensity;
                if (distance <= _minDistance)
                {
                    intensity = 1f;
                }
                else
                {
                    intensity = 1f - Mathf.Clamp01(
                        (distance - _minDistance) / (_detectionRadius - _minDistance)
                    );
                }
                
                float alpha = Mathf.Lerp(0.2f, 1f, intensity);
                Color lineColor = Color.Lerp(
                    new Color(1f, 0.8f, 0f, alpha),
                    new Color(1f, 0f, 0f, alpha), 
                    intensity
                );

                Gizmos.color = lineColor;
                Gizmos.DrawLine(agentPos, obstaclePos);
                
                float sphereSize = Mathf.Lerp(0.15f, 0.5f, intensity);
                Gizmos.DrawSphere(obstaclePos, sphereSize);
                
                if (distance <= _minDistance)
                {
                    Gizmos.color = Color.red;
                    float crossSize = 0.4f;
                    Gizmos.DrawLine(
                        obstaclePos + new Vector3(-crossSize, 0, -crossSize),
                        obstaclePos + new Vector3(crossSize, 0, crossSize)
                    );
                    Gizmos.DrawLine(
                        obstaclePos + new Vector3(-crossSize, 0, crossSize),
                        obstaclePos + new Vector3(crossSize, 0, -crossSize)
                    );
                }

#if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    obstaclePos + Vector3.up * 1.2f,
                    $"d={distance:F1} danger={intensity:F2}"
                );
#endif
            }
            else
            {
                Gizmos.color = _safeConnectionColor;
                Gizmos.DrawLine(agentPos, obstaclePos);
                Gizmos.DrawWireSphere(obstaclePos, 0.1f);
            }
        }
    }

    private void DrawCircleXZ(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0f,
                Mathf.Sin(angle) * radius
            );
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}