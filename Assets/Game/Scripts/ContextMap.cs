using UnityEngine;

public class ContextMap
{
    public float[] Values => _values;
    public int Resolution => Values.Length;

    private readonly float[] _values;

    public ContextMap(int resolution)
    {
        _values = new float[resolution];
    }
    
    public Vector2 GetDirection(int slotIndex)
    {
        float angle = slotIndex * (360f / Resolution);
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }
    
    public int GetSlotIndex(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0f) 
            angle += 360f;
        float slotSize = 360f / Resolution;
        int index = Mathf.RoundToInt(angle / slotSize) % Resolution;
        return index;
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
            _values[slot] = Mathf.Max(_values[slot], value);
        }
    }

    public void Clear()
    {
        for (int i = 0; i < Resolution; i++)
        {
            _values[i] = 0f;
        }
    }
    
    public void MergeMax(ContextMap other)
    {
        for (int i = 0; i < Resolution; i++)
        {
            _values[i] = Mathf.Max(_values[i], other._values[i]);
        }
    }
    
    public void ApplyMask(bool[] mask)
    {
        for (int i = 0; i < Resolution; i++)
        {
            if (mask[i])
            {
                _values[i] = 0f;
            }
        }
    }
}
