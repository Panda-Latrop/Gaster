using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaPlayerTeleportComponent : CinemaBaseComponent
{
    [SerializeField]
    protected Vector3 point;

    public override void OnPlay()
    {
        base.OnPlay();
        GameInstance.Instance.PlayerController.ControlledPawn.transform.position = point;
    }
    public override bool OnUpdate()
    {
        return true;
    }
    public override void OnSkip()
    {
        OnPlay();
        base.OnSkip();
    }
}
