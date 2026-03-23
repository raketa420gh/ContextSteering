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

    public void EvaluateInterest(ContextMap interestMap, Transform agentTransform)
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag(_attractionTag);
        Vector2 agentPos = ToVec2(agentTransform.position);

        foreach (GameObject point in points)
        {
            Vector2 pointPos = ToVec2(point.transform.position);
            Vector2 toPoint = pointPos - agentPos;
            float distance = toPoint.magnitude;

            if (distance > _maxDistance || distance < 0.5f)
                continue;

            float normalizedProximity = 1f - Mathf.Clamp01(distance / _maxDistance);
            float intensity = normalizedProximity * _maxIntensity;

            interestMap.WriteValue(toPoint.normalized, intensity, _falloffSlots);
        }
    }

    private Vector2 ToVec2(Vector3 v) => new Vector2(v.x, v.z);
}