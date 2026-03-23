using UnityEngine;

[RequireComponent(typeof(ContextSteeringAgent))]
public class ContextSteeringDebugVisualizer : MonoBehaviour
{
    [SerializeField] 
    private float _rayLength = 2f;
    [SerializeField] 
    private bool _showInterest = true;
    [SerializeField] 
    private bool _showDanger = true;

    private ContextSteeringBrain _brain;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        var agent = GetComponent<ContextSteeringAgent>();
        if (agent == null) 
            return;

        ContextMap interest = agent.DebugInterest;
        ContextMap danger = agent.DebugDanger;

        if (interest == null || danger == null)
            return;

        Vector3 origin = transform.position + Vector3.up * 0.1f;

        for (int i = 0; i < interest.Resolution; i++)
        {
            Vector2 dir2D = interest.GetDirection(i);
            Vector3 dir3D = new Vector3(dir2D.x, 0f, dir2D.y);

            if (_showInterest && interest.Values[i] > 0.001f)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(origin, dir3D * interest.Values[i] * _rayLength);
            }

            if (_showDanger && danger.Values[i] > 0.001f)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(origin, dir3D * danger.Values[i] * _rayLength);
            }
        }
    }
}