using UnityEngine;

public class AttractionPointBehavior : MonoBehaviour, IInterestBehavior
{
    [SerializeField]
    private float _maxDistance = 15f;

    [SerializeField]
    private float _falloffSlots = 4f;

    [SerializeField]
    private float _maxIntensity = 0.5f;

    private ContextSteeringAgent _agent;

    public void EvaluateInterest(ContextMap interestMap, Transform agentTransform)
    {
        AttractionComponent[] attractions = Object.FindObjectsOfType<AttractionComponent>();

        if (attractions == null || attractions.Length == 0)
            return;

        if (_agent == null)
            _agent = agentTransform.GetComponent<ContextSteeringAgent>();

        float agentRadius = _agent != null ? _agent.Radius : 0f;
        Vector2 agentPos = new Vector2(agentTransform.position.x, agentTransform.position.z);

        foreach (AttractionComponent attraction in attractions)
        {
            if (attraction == null)
                continue;

            Vector2 toAttraction = attraction.Position2D - agentPos;
            float distance = toAttraction.magnitude;
            float arrivalDistance = attraction.Radius + agentRadius;

            if (distance > _maxDistance || distance < arrivalDistance)
                continue;

            float normalizedProximity = 1f - Mathf.Clamp01(distance / _maxDistance);
            float intensity = Mathf.SmoothStep(0f, _maxIntensity, normalizedProximity);

            interestMap.WriteValue(toAttraction.normalized, intensity, _falloffSlots);
        }
    }
}
