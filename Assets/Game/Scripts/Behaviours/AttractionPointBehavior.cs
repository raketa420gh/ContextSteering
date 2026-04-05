using UnityEngine;

public class AttractionPointBehavior : MonoBehaviour, IInterestBehavior
{
    [SerializeField]
    private string _attractionTag = "Attraction";

    [SerializeField]
    private float _maxDistance = 15f;

    [SerializeField]
    private float _falloffSlots = 4f;

    [SerializeField]
    private float _maxIntensity = 0.5f;

    private GameObject[] _cachedPoints;
    private float _cacheTime;
    private const float CacheRefreshInterval = 1f;

    public void EvaluateInterest(ContextMap interestMap, Transform agentTransform)
    {
        RefreshAttractionCache();

        if (_cachedPoints == null || _cachedPoints.Length == 0)
            return;

        Vector2 agentPos = ToVec2(agentTransform.position);

        foreach (GameObject point in _cachedPoints)
        {
            if (point == null)
                continue;

            EvaluateAttractionPoint(interestMap, agentPos, point);
        }
    }

    private void RefreshAttractionCache()
    {
        if (_cachedPoints == null || Time.time - _cacheTime > CacheRefreshInterval)
        {
            _cachedPoints = GameObject.FindGameObjectsWithTag(_attractionTag);
            _cacheTime = Time.time;
        }
    }

    private void EvaluateAttractionPoint(ContextMap interestMap, Vector2 agentPos, GameObject point)
    {
        Vector2 pointPos = ToVec2(point.transform.position);
        Vector2 toPoint = pointPos - agentPos;
        float distance = toPoint.magnitude;

        if (distance > _maxDistance || distance < 0.5f)
            return;

        float normalizedProximity = 1f - Mathf.Clamp01(distance / _maxDistance);
        float intensity = Mathf.SmoothStep(0f, _maxIntensity, normalizedProximity);

        interestMap.WriteValue(toPoint.normalized, intensity, _falloffSlots);
    }

    private Vector2 ToVec2(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
}