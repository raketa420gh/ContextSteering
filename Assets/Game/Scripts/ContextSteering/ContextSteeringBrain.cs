using System.Collections.Generic;
using UnityEngine;

public class ContextSteeringBrain
{
    private readonly int _resolution;
    private readonly float _temporalBlend;

    private readonly ContextMap _combinedInterest;
    private readonly ContextMap _combinedDanger;
    private readonly ContextMap _previousInterest;
    private readonly ContextMap _previousDanger;
    private readonly ContextMap _tempInterest;
    private readonly ContextMap _tempDanger;

    public ContextMap CombinedInterest => _combinedInterest;
    public ContextMap CombinedDanger => _combinedDanger;

    public ContextSteeringBrain(int resolution, float temporalBlend)
    {
        _resolution = resolution;
        _temporalBlend = temporalBlend;

        _combinedInterest = new ContextMap(resolution);
        _combinedDanger = new ContextMap(resolution);
        _previousInterest = new ContextMap(resolution);
        _previousDanger = new ContextMap(resolution);
        _tempInterest = new ContextMap(resolution);
        _tempDanger = new ContextMap(resolution);
    }

    public void Evaluate(IReadOnlyList<IInterestBehavior> interestBehaviors, 
        IReadOnlyList<IDangerBehavior> dangerBehaviors, Transform agentTransform)
    {
        _combinedInterest.Clear();
        _combinedDanger.Clear();

        foreach (IInterestBehavior behavior in interestBehaviors)
        {
            _tempInterest.Clear();
            behavior.EvaluateInterest(_tempInterest, agentTransform);
            _combinedInterest.MergeMax(_tempInterest);
        }

        foreach (IDangerBehavior behavior in dangerBehaviors)
        {
            _tempDanger.Clear();
            behavior.EvaluateDanger(_tempDanger, agentTransform);
            _combinedDanger.MergeMax(_tempDanger);
        }
        
        if (_temporalBlend > 0.001f)
        {
            _combinedInterest.BlendWith(_previousInterest, _temporalBlend);
            _combinedDanger.BlendWith(_previousDanger, _temporalBlend);
        }

        _previousInterest.CopyFrom(_combinedInterest);
        _previousDanger.CopyFrom(_combinedDanger);
    }
}