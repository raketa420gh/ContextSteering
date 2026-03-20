using System.Collections.Generic;
using UnityEngine;

public class ContextMapDebugVisualizer : MonoBehaviour
{
    [SerializeField] 
    private bool _showInterest = true;
    [SerializeField] 
    private bool _showDanger = true;
    [SerializeField] 
    private float _rayLength = 3f;

    private ContextSteeringAgent _agent;
    private ContextMap _debugInterest;
    private ContextMap _debugDanger;
    private ContextSteeringResolver _debugResolver;
    private List<IContextBehavior> _behaviors;
    private int _resolution;

    private void Awake()
    {
        _agent = GetComponent<ContextSteeringAgent>();
        _behaviors = new List<IContextBehavior>(GetComponents<IContextBehavior>());
        
        _resolution = 16;
    }

    private void Start()
    {
        _debugInterest = new ContextMap(_resolution);
        _debugDanger = new ContextMap(_resolution);
        _debugResolver = new ContextSteeringResolver();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (_debugInterest == null) return;

        _debugInterest.Clear();
        _debugDanger.Clear();

        ContextMap tempI = new ContextMap(_resolution);
        ContextMap tempD = new ContextMap(_resolution);

        foreach (IContextBehavior b in _behaviors)
        {
            tempI.Clear();
            tempD.Clear();
            b.EvaluateInterest(tempI, transform);
            b.EvaluateDanger(tempD, transform);
            _debugInterest.MergeMax(tempI);
            _debugDanger.MergeMax(tempD);
        }

        Vector3 origin = transform.position + Vector3.up * 0.1f;

        for (int i = 0; i < _resolution; i++)
        {
            Vector2 dir2D = _debugInterest.GetDirection(i);
            Vector3 dir3D = new Vector3(dir2D.x, 0f, dir2D.y);

            if (_showInterest)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(origin, dir3D * _debugInterest.Values[i] * _rayLength);
            }

            if (_showDanger)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(origin, dir3D * _debugDanger.Values[i] * _rayLength);
            }
        }
        
        Vector2 finalDir = _debugResolver.Resolve(_debugInterest, _debugDanger, out float spd);
        if (finalDir.sqrMagnitude > 0.001f)
        {
            Gizmos.color = Color.yellow;
            Vector3 finalDir3D = new Vector3(finalDir.x, 0f, finalDir.y);
            Gizmos.DrawRay(origin, finalDir3D * spd * _rayLength * 1.5f);
        }
    }
}