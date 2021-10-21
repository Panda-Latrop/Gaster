using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTrajectory : MonoBehaviour
{
    public WeaponTrajectoryBaseComponent traectori;

    public List<Vector2> points = new List<Vector2>(5);

    public void Start()
    {
        traectori.SetCount(points.Count);
    }
    public void Update()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i] = Vector2.Lerp(points[i],traectori.Evaluate(i),0.45f);
        }
        traectori.OnUpdate();
    }
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(points[i], 1);
            }
        }
    }
}
