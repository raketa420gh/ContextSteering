using UnityEngine;

public interface IInterestBehavior
{
    void EvaluateInterest(ContextMap interestMap, Transform agentTransform);
}