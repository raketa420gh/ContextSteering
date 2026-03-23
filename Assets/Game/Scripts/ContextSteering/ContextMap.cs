using UnityEngine;

public class ContextMap
{
    public float[] Values { get; }
    public int Resolution => Values.Length;

    public ContextMap(int resolution)
    {
        Values = new float[resolution];
    }

    public Vector2 GetDirection(int slotIndex)
    {
        float angle = slotIndex * (360f / Resolution);
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    public int GetSlotIndex(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0f)
            angle += 360f;
        float slotSize = 360f / Resolution;
        return Mathf.RoundToInt(angle / slotSize) % Resolution;
    }

    public void WriteValue(Vector2 direction, float intensity, float falloffSlots)
    {
        int centerSlot = GetSlotIndex(direction);
        int halfSpread = Mathf.CeilToInt(falloffSlots);

        for (int offset = -halfSpread; offset <= halfSpread; offset++)
        {
            int slot = (centerSlot + offset + Resolution) % Resolution;
            float t = Mathf.Abs(offset) / Mathf.Max(falloffSlots, 0.001f);
            float value = intensity * Mathf.Clamp01(1f - t);
            Values[slot] = Mathf.Max(Values[slot], value);
        }
    }

    public void Clear()
    {
        System.Array.Clear(Values, 0, Resolution);
    }

    public void MergeMax(ContextMap other)
    {
        for (int i = 0; i < Resolution; i++)
            Values[i] = Mathf.Max(Values[i], other.Values[i]);
    }

    public void BlendWith(ContextMap previous, float blendFactor)
    {
        for (int i = 0; i < Resolution; i++)
            Values[i] = Mathf.Lerp(Values[i], previous.Values[i], blendFactor);
    }

    public void CopyFrom(ContextMap source)
    {
        System.Array.Copy(source.Values, Values, Resolution);
    }
}