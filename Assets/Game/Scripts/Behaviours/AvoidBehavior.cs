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

    private ObstacleComponent[] _cachedObstacles;
    private float _cacheTime;
    private const float CacheRefreshInterval = 0.5f;

    public void EvaluateDanger(ContextMap dangerMap, Transform agentTransform)
    {
        RefreshObstacleCache();

        if (_cachedObstacles == null || _cachedObstacles.Length == 0)
            return;

        Vector2 agentPos = ToVec2(agentTransform.position);

        foreach (ObstacleComponent obstacle in _cachedObstacles)
        {
            if (obstacle == null)
                continue;

            ObstacleData data = obstacle.GetData();
            EvaluateObstacle(dangerMap, agentPos, data);
        }
    }

    private void RefreshObstacleCache()
    {
        if (_cachedObstacles == null || Time.time - _cacheTime > CacheRefreshInterval)
        {
            _cachedObstacles = FindObjectsOfType<ObstacleComponent>();
            _cacheTime = Time.time;
        }
    }

    private void EvaluateObstacle(ContextMap dangerMap, Vector2 agentPos, ObstacleData obstacle)
    {
        Vector2 toObstacle = obstacle.Position - agentPos;
        float distance = toObstacle.magnitude;

        float threatRadius = obstacle.Radius + _safetyMargin;

        if (distance > _detectionRadius || distance < 0.01f)
            return;

        float intensity = CalculateIntensity(distance, threatRadius);
        float effectiveFalloff = CalculateEffectiveFalloff(distance, threatRadius);

        dangerMap.WriteValue(toObstacle.normalized, intensity, effectiveFalloff);
    }

    private float CalculateIntensity(float distance, float threatRadius)
    {
        if (distance <= threatRadius)
            return _maxIntensity;

        float dangerZone = _detectionRadius - threatRadius;
        if (dangerZone < 0.01f)
            return _maxIntensity;

        float t = 1f - Mathf.Clamp01((distance - threatRadius) / dangerZone);
        return Mathf.SmoothStep(0f, _maxIntensity, t);
    }

    private float CalculateEffectiveFalloff(float distance, float threatRadius)
    {
        float proximityFactor = Mathf.Clamp01(threatRadius / Mathf.Max(distance, 0.01f));
        return _falloffSlots * (1f + proximityFactor * 2f);
    }

    private Vector2 ToVec2(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
}