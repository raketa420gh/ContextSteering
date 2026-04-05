using UnityEngine;

public class ObstacleData
{
    public Vector2 Position { get; }
    public float Radius { get; }

    public ObstacleData(Vector2 position, float radius)
    {
        Position = position;
        Radius = radius;
    }
}