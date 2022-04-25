using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaObstacle : Actor
{
    [SerializeField]
    protected new Collider2D collider;

    public virtual void Show()
    {
        collider.enabled = true;
    }
    public virtual void Hide()
    {
        collider.enabled = false;
    }
}
