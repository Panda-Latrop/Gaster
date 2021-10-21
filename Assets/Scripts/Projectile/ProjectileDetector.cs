using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDetector : MonoBehaviour
{
    [SerializeField]
    protected ProjectileBase projectile;
    [SerializeField]
    protected Collider2D detector;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        projectile.CheckTarget(collision);
    }
}
