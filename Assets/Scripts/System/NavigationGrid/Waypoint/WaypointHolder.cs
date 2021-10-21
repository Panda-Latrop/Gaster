using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointHolder : MonoBehaviour
{
    [SerializeField]
    protected bool looped;
    [SerializeField]
    protected List<Vector3> points = new List<Vector3>();

    public bool Looped => looped;
    //public List<Vector3> Points => points;
    public void GetPath(List<Vector3> path)
    {
        path.Clear();
        for (int i = 0; i < points.Count; i++)
        {
            path.Add(points[i] + transform.position);
        }
    }
    public Vector3 Get(int id) => points[id] + transform.position;
    public Vector3 Set(int id,Vector3 point) => points[id] = point- transform.position;
    public int Length => points.Count;

    protected void OnDrawGizmosSelected()
    {
        if (points.Count > 0)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(points[0] + transform.position, 0.25f);
            for (int i = 1; i < points.Count; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(points[i] + transform.position, 0.25f);
                Gizmos.DrawLine(points[i - 1] + transform.position, points[i] + transform.position);
                Gizmos.DrawSphere(points[i] + (points[i - 1] - points[i]).normalized * 0.25f + transform.position, 0.1f);
            }
            if (looped && points.Count > 1)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(points[0] + transform.position, points[points.Count - 1]);
                Gizmos.DrawSphere(points[0] + (points[points.Count - 1] - points[0]).normalized * 0.25f + transform.position, 0.1f);
            }
        }
    }
}
