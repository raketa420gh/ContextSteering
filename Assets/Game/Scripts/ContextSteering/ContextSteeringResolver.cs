using UnityEngine;

public class ContextSteeringResolver
{
    private readonly float _dangerThreshold;
    private readonly float _minimumSpeedThreshold;

    public ContextSteeringResolver(float dangerThreshold, float minimumSpeedThreshold)
    {
        _dangerThreshold = dangerThreshold;
        _minimumSpeedThreshold = minimumSpeedThreshold;
    }

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
            bool isDangerous = dangerMap.Values[i] > minDanger + _dangerThreshold;
            maskedInterest[i] = isDangerous ? 0f : interestMap.Values[i];
        }
        
        int bestSlot = FindBestSlot(maskedInterest, out float bestValue);

        speed = CalculateSpeed(bestValue, dangerMap, bestSlot);

        if (bestSlot < 0 || bestValue < 0.001f)
            return Vector2.zero;
        
        return SubslotRefinement(maskedInterest, bestSlot, resolution);
    }

    private int FindBestSlot(float[] maskedInterest, out float bestValue)
    {
        int bestSlot = -1;
        bestValue = 0f;
        
        for (int i = 0; i < maskedInterest.Length; i++)
        {
            if (maskedInterest[i] > bestValue)
            {
                bestValue = maskedInterest[i];
                bestSlot = i;
            }
        }

        return bestSlot;
    }

    private float CalculateSpeed(float interest, ContextMap dangerMap, int slotIndex)
    {
        if (slotIndex < 0)
            return 0f;

        float dangerPenalty = 1f - Mathf.Clamp01(dangerMap.Values[slotIndex]);
        float finalSpeed = interest * dangerPenalty;

        return Mathf.Max(finalSpeed, interest > 0.001f ? _minimumSpeedThreshold : 0f);
    }

    private Vector2 SubslotRefinement(float[] maskedInterest, int bestSlot, int resolution)
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