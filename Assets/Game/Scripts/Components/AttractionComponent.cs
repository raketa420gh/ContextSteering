using UnityEngine;

public class AttractionComponent : MonoBehaviour
{
    public float Radius => _radius;
    public Vector2 Position2D => new Vector2(transform.position.x, transform.position.z);

    [SerializeField]
    private float _radius = 1f;

    [SerializeField]
    private Color _gizmoColor = Color.cyan;

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
