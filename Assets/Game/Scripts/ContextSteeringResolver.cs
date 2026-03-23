using UnityEngine;

public class ContextSteeringResolver
{
    public Vector2 Resolve(ContextMap interestMap, ContextMap dangerMap, out float speed)
    {
        int resolution = interestMap.Resolution;
        
        float minDanger = float.MaxValue;
        for (int i = 0; i < resolution; i++)
        {
            if (dangerMap.Values[i] < minDanger)
                minDanger = dangerMap.Values[i];
        }
        
        float[] maskedInterest = new float[resolution];
        for (int i = 0; i < resolution; i++)
        {
            bool isDangerous = dangerMap.Values[i] > minDanger + 0.01f;
            maskedInterest[i] = isDangerous ? 0f : interestMap.Values[i];
        }
        
        int bestSlot = -1;
        float bestValue = 0f;
        for (int i = 0; i < resolution; i++)
        {
            if (maskedInterest[i] > bestValue)
            {
                bestValue = maskedInterest[i];
                bestSlot = i;
            }
        }

        speed = bestValue;

        if (bestSlot < 0 || bestValue < 0.001f)
            return Vector2.zero;
        
        return SubslotRefinement(interestMap, maskedInterest, bestSlot, resolution);
    }

    private Vector2 SubslotRefinement(ContextMap map, float[] maskedInterest, int bestSlot, int resolution)
    {
        int prevSlot = (bestSlot - 1 + resolution) % resolution;
        int nextSlot = (bestSlot + 1) % resolution;

        float prevValue = maskedInterest[prevSlot];
        float bestValue = maskedInterest[bestSlot];
        float nextValue = maskedInterest[nextSlot];

        float offset = 0f;
        float total = prevValue + nextValue;
        if (total > 0.001f)
        {
            offset = (nextValue - prevValue) / (2f * Mathf.Max(bestValue, 0.001f));
            offset = Mathf.Clamp(offset, -0.5f, 0.5f);
        }

        float virtualSlot = bestSlot + offset;
        float angle = virtualSlot * (360f / resolution);
        float rad = angle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }
}