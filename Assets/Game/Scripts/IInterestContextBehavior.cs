using UnityEngine;

public interface IInterestContextBehavior
{
    void EvaluateInterest(ContextMap interestMap, Transform agentTransform);
}