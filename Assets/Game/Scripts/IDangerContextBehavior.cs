using UnityEngine;

public interface IDangerContextBehavior
{
    void EvaluateDanger(ContextMap dangerMap, Transform agentTransform);
}