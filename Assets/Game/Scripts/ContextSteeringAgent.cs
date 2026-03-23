using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MoveComponent))]
public class ContextSteeringAgent : MonoBehaviour
{
    public ContextMap DebugInterest => _brain?.CombinedInterest;
    public ContextMap DebugDanger => _brain?.CombinedDanger;
    
    [SerializeField] 
    private int _mapResolution = 16;
    
    [SerializeField] 
    [Range(0f, 0.95f)] 
    float _temporalBlend = 0.3f;

    private List<IInterestBehavior> _interestBehaviors;
    private List<IDangerBehavior> _dangerBehaviors;
    private ContextSteeringBrain _brain;
    private ContextSteeringResolver _resolver;
    private MoveComponent _mover;

    private void Awake()
    {
        _interestBehaviors = GetComponents<IInterestBehavior>().ToList();
        _dangerBehaviors = GetComponents<IDangerBehavior>().ToList();

        _brain = new ContextSteeringBrain(_mapResolution, _temporalBlend);
        _resolver = new ContextSteeringResolver();
        _mover = GetComponent<MoveComponent>();
    }

    private void Update()
    {
        _brain.Evaluate(_interestBehaviors, _dangerBehaviors, transform);

        Vector2 direction = _resolver.Resolve(
            _brain.CombinedInterest,
            _brain.CombinedDanger,
            out float speed
        );

        _mover.Move(direction, speed);
    }
}