using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerControllerBase : ControllerBase
{
    public bool gameInput = true;
    public override void Possess(Pawn pawn)
    {
        base.Possess(pawn);
        controlledPawn.SaveTag = "next";
    }
    public abstract void SetStart(Vector3 position, Quaternion rotation);
}
