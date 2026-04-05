using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MoveComponent))]
public class ContextSteeringAgent : MonoBehaviour
{
    public ContextMap DebugInterest => _brain?.CombinedInterest;
    public ContextMap DebugDanger => _brain?.CombinedDanger;

    [SerializeField]
    private ContextSteeringConfig _config;

    private List<IInterestBehavior> _interestBehaviors;
    private List<IDangerBehavior> _dangerBehaviors;
    private ContextSteeringBrain _brain;
    private ContextSteeringResolver _resolver;
    private MoveComponent _mover;
    private bool _isInitialized;

    private void Awake()
    {
        if (!ValidateConfiguration())
            return;

        InitializeBehaviors();
        InitializeComponents();
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        _brain.Evaluate(_interestBehaviors, _dangerBehaviors, transform);

        Vector2 direction = _resolver.Resolve(
            _brain.CombinedInterest,
            _brain.CombinedDanger,
            out float speed
        );

        _mover.Move(direction, speed);
    }

    private bool ValidateConfiguration()
    {
        if (_config == null)
        {
            Debug.LogError($"[{gameObject.name}] ContextSteeringConfig is not assigned!", this);
            enabled = false;
            return false;
        }

        return true;
    }

    private void InitializeBehaviors()
    {
        _interestBehaviors = GetComponents<IInterestBehavior>().ToList();
        _dangerBehaviors = GetComponents<IDangerBehavior>().ToList();

        if (_interestBehaviors.Count == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] No interest behaviors found. Agent won't move.", this);
        }
    }

    private void InitializeComponents()
    {
        _brain = new ContextSteeringBrain(_config.MapResolution, _config.TemporalBlend);
        _resolver = new ContextSteeringResolver(_config.DangerThreshold, _config.MinimumSpeedThreshold);
        _mover = GetComponent<MoveComponent>();

        if (_mover == null)
        {
            Debug.LogError($"[{gameObject.name}] MoveComponent is missing!", this);
            enabled = false;
        }
    }
}