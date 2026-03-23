using UnityEngine;

public interface IDangerBehavior
{
    void EvaluateDanger(ContextMap dangerMap, Transform agentTransform);
}