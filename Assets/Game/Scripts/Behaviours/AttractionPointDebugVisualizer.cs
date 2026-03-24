using UnityEngine;

[RequireComponent(typeof(AttractionPointBehavior))]
public class AttractionPointDebugVisualizer : MonoBehaviour
{
    [Header("Gizmos Settings")]
    [SerializeField] 
    private bool _showGizmos = true;
    [SerializeField] 
    private bool _showConnections = true;
    [SerializeField] 
    private bool _showRadius = true;
    [SerializeField] 
    private Color _radiusColor = new Color(0f, 1f, 0.5f, 0.15f);
    [SerializeField] 
    private Color _connectionColor = new Color(0f, 1f, 0.5f, 0.6f);
    [SerializeField] 
    private Color _outOfRangeColor = new Color(1f, 1f, 1f, 0.15f);

    [Header("References (auto-filled)")]
    [SerializeField] 
    private string _attractionTag = "Attraction";
    [SerializeField] 
    private float _maxDistance = 15f;

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
        var behavior = GetComponent<AttractionPointBehavior>();
        if (behavior == null) return;

        var so = new UnityEditor.SerializedObject(behavior);
        var tagProp = so.FindProperty("_attractionTag");
        var distProp = so.FindProperty("_maxDistance");

        if (tagProp != null) _attractionTag = tagProp.stringValue;
        if (distProp != null) _maxDistance = distProp.floatValue;
    }

    private void OnDrawGizmosSelected()
    {
        if (!_showGizmos) return;

        Vector3 agentPos = transform.position;
        
        if (_showRadius)
        {
            Gizmos.color = _radiusColor;
            DrawCircleXZ(agentPos, _maxDistance, 64);
            
            UnityEditor.Handles.color = _radiusColor;
            UnityEditor.Handles.DrawSolidDisc(agentPos, Vector3.up, _maxDistance);
            
            Gizmos.color = new Color(_radiusColor.r, _radiusColor.g, _radiusColor.b, 0.5f);
            DrawCircleXZ(agentPos, _maxDistance, 64);
        }

        if (!_showConnections) return;
        
        GameObject[] points;
        try
        {
            points = GameObject.FindGameObjectsWithTag(_attractionTag);
        }
        catch
        {
            return;
        }

        Vector2 agentPos2D = new Vector2(agentPos.x, agentPos.z);

        foreach (GameObject point in points)
        {
            if (point == this.gameObject) continue;

            Vector3 pointPos = point.transform.position;
            Vector2 pointPos2D = new Vector2(pointPos.x, pointPos.z);
            float distance = (pointPos2D - agentPos2D).magnitude;

            if (distance < 0.5f) continue;

            bool inRange = distance <= _maxDistance;

            if (inRange)
            {
                float normalizedProximity = 1f - Mathf.Clamp01(distance / _maxDistance);
                float alpha = Mathf.Lerp(0.15f, 0.9f, normalizedProximity);

                Gizmos.color = new Color(
                    _connectionColor.r,
                    _connectionColor.g,
                    _connectionColor.b,
                    alpha
                );
                
                Gizmos.DrawLine(agentPos, pointPos);
                
                float sphereSize = Mathf.Lerp(0.2f, 0.6f, normalizedProximity);
                Gizmos.DrawSphere(pointPos, sphereSize);

#if UNITY_EDITOR
                float intensity = normalizedProximity;
                UnityEditor.Handles.Label(
                    pointPos + Vector3.up * 1.2f,
                    $"d={distance:F1} int={intensity:F2}"
                );
#endif
            }
            else
            {
                Gizmos.color = _outOfRangeColor;
                Gizmos.DrawLine(agentPos, pointPos);
                Gizmos.DrawWireSphere(pointPos, 0.15f);
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