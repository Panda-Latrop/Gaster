using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArena : TriggerStart
{
    [SerializeField]
    protected ArenaMaster arena;
    protected override void Start()
    {
        arena.BindOnEnd(Execute);
        enabled = false;
    }
}
