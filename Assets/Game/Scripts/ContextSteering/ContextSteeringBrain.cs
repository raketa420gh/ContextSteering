using System.Collections.Generic;
using UnityEngine;

public class ContextSteeringBrain
{
    public ContextMap CombinedInterest => _combinedInterest;
    public ContextMap CombinedDanger => _combinedDanger;
    
    private readonly int _resolution;
    private readonly float _temporalBlend;

    private readonly ContextMap _combinedInterest;
    private readonly ContextMap _combinedDanger;
    private readonly ContextMap _previousInterest;
    private readonly ContextMap _previousDanger;
    private readonly ContextMap _tempInterest;
    private readonly ContextMap _tempDanger;

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

        if (interestBehaviors != null && interestBehaviors.Count > 0)
        {
            EvaluateInterestBehaviors(interestBehaviors, agentTransform);
        }

        if (dangerBehaviors != null && dangerBehaviors.Count > 0)
        {
            EvaluateDangerBehaviors(dangerBehaviors, agentTransform);
        }
        
        ApplyPostProcessing();
    }

    private void EvaluateInterestBehaviors(IReadOnlyList<IInterestBehavior> behaviors, Transform agentTransform)
    {
        foreach (IInterestBehavior behavior in behaviors)
        {
            if (behavior == null)
                continue;

            _tempInterest.Clear();
            
            try
            {
                behavior.EvaluateInterest(_tempInterest, agentTransform);
                _combinedInterest.MergeMax(_tempInterest);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error evaluating interest behavior {behavior.GetType().Name}: {e.Message}");
            }
        }
    }

    private void EvaluateDangerBehaviors(IReadOnlyList<IDangerBehavior> behaviors, Transform agentTransform)
    {
        foreach (IDangerBehavior behavior in behaviors)
        {
            if (behavior == null)
                continue;

            _tempDanger.Clear();
            
            try
            {
                behavior.EvaluateDanger(_tempDanger, agentTransform);
                _combinedDanger.MergeMax(_tempDanger);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error evaluating danger behavior {behavior.GetType().Name}: {e.Message}");
            }
        }
    }

    private void ApplyPostProcessing()
    {
        _combinedInterest.ApplyBlur(1);
        _combinedDanger.ApplyBlur(1);

        if (_temporalBlend > 0.001f)
        {
            _combinedInterest.BlendWith(_previousInterest, _temporalBlend);
            _combinedDanger.BlendWith(_previousDanger, _temporalBlend);
        }

        _previousInterest.CopyFrom(_combinedInterest);
        _previousDanger.CopyFrom(_combinedDanger);
    }
}