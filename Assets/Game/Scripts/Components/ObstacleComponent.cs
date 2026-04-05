using UnityEngine;

public class ObstacleComponent : MonoBehaviour
{
    public float Radius => _radius;
    public Vector2 Position2D => new Vector2(transform.position.x, transform.position.z);
    
    [SerializeField]
    private float _radius = 1f;

    [SerializeField]
    private Color _gizmoColor = Color.red;

    public ObstacleData GetData()
    {
        return new ObstacleData(Position2D, _radius);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Vector3 pos = transform.position;
        Gizmos.DrawWireSphere(pos, _radius);
    }
}