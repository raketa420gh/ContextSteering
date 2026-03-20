using System.Collections.Generic;
using UnityEngine;

public class ContextSteeringAgent : MonoBehaviour
{
    [SerializeField] 
    private int _mapResolution = 16;
    [SerializeField] 
    private float _moveSpeed = 5f;
    [SerializeField] 
    private float _rotationSmoothing = 10f;
    
    [Tooltip("Сглаживание через blend с предыдущим кадром (0 = нет, 0.9 = сильное)")]
    [SerializeField] 
    [Range(0f, 0.95f)] 
    private float _temporalBlend = 0.5f;

    private List<IContextBehavior> _behaviors;
    private ContextMap _combinedInterest;
    private ContextMap _combinedDanger;
    private ContextMap _previousInterest;
    private ContextMap _previousDanger;
    private ContextSteeringResolver _resolver;

    private void Awake()
    {
        _behaviors = new List<IContextBehavior>(GetComponents<IContextBehavior>());

        _combinedInterest = new ContextMap(_mapResolution);
        _combinedDanger = new ContextMap(_mapResolution);
        _previousInterest = new ContextMap(_mapResolution);
        _previousDanger = new ContextMap(_mapResolution);
        _resolver = new ContextSteeringResolver();
    }

    private void Update()
    {
        _combinedInterest.Clear();
        _combinedDanger.Clear();
        
        ContextMap tempInterest = new ContextMap(_mapResolution);
        ContextMap tempDanger = new ContextMap(_mapResolution);
        
        foreach (IContextBehavior behavior in _behaviors)
        {
            tempInterest.Clear();
            tempDanger.Clear();

            behavior.EvaluateInterest(tempInterest, transform);
            behavior.EvaluateDanger(tempDanger, transform);

            _combinedInterest.MergeMax(tempInterest);
            _combinedDanger.MergeMax(tempDanger);
        }
        
        ApplyTemporalBlend(_combinedInterest, _previousInterest);
        ApplyTemporalBlend(_combinedDanger, _previousDanger);
        
        CopyMap(_combinedInterest, _previousInterest);
        CopyMap(_combinedDanger, _previousDanger);
        
        Vector2 direction = _resolver.Resolve(_combinedInterest, _combinedDanger, out float speed);
        
        if (direction.sqrMagnitude > 0.001f)
        {
            Vector3 moveDir = new Vector3(direction.x, 0f, direction.y);
            Vector3 velocity = moveDir * speed * _moveSpeed;

            transform.position += velocity * Time.deltaTime;
            
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * _rotationSmoothing
            );
        }
    }

    private void ApplyTemporalBlend(ContextMap current, ContextMap previous)
    {
        for (int i = 0; i < current.Resolution; i++)
        {
            current.Values[i] = Mathf.Lerp(
                current.Values[i],
                previous.Values[i],
                _temporalBlend
            );
        }
    }

    private void CopyMap(ContextMap source, ContextMap destination)
    {
        for (int i = 0; i < source.Resolution; i++)
        {
            destination.Values[i] = source.Values[i];
        }
    }
}