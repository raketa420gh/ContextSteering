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

        if (points == null || points.Length == 0)
            return;

        Vector2 agentPos = new Vector2(agentTransform.position.x, agentTransform.position.z);

        foreach (GameObject point in points)
        {
            if (point == null)
                continue;

            Vector2 pointPos = new Vector2(point.transform.position.x, point.transform.position.z);
            Vector2 toPoint = pointPos - agentPos;
            float distance = toPoint.magnitude;

            if (distance > _maxDistance || distance < 0.5f)
                continue;

            float normalizedProximity = 1f - Mathf.Clamp01(distance / _maxDistance);
            float intensity = Mathf.SmoothStep(0f, _maxIntensity, normalizedProximity);

            interestMap.WriteValue(toPoint.normalized, intensity, _falloffSlots);
        }
    }
}