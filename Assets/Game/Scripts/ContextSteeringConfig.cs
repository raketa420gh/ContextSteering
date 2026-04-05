using UnityEngine;

[CreateAssetMenu(fileName = "ContextSteeringConfig", menuName = "AI/Context Steering Config")]
public class ContextSteeringConfig : ScriptableObject
{
    public int MapResolution => _mapResolution;
    public float TemporalBlend => _temporalBlend;
    public float MoveSpeed => _moveSpeed;
    public float RotationSmoothing => _rotationSmoothing;
    public float MinimumSpeedThreshold => _minimumSpeedThreshold;
    public float DangerThreshold => _dangerThreshold;
    
    [Header("Map Settings")]
    [SerializeField]
    private int _mapResolution = 16;

    [SerializeField]
    [Range(0f, 0.95f)]
    private float _temporalBlend = 0.3f;

    [Header("Movement")]
    [SerializeField]
    private float _moveSpeed = 5f;

    [SerializeField]
    private float _rotationSmoothing = 10f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _minimumSpeedThreshold = 0.1f;

    [Header("Danger Processing")]
    [SerializeField]
    [Range(0f, 1f)]
    private float _dangerThreshold = 0.01f;
}