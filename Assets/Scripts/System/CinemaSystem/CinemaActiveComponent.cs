using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaActiveComponent : CinemaBaseComponent
{
    public enum CinemaActiveObjectType
    {
        target,
        player,
    }

    [SerializeField]
    protected bool active;
    [SerializeField]
    protected CinemaActiveObjectType type;
    [SerializeField]
    protected GameObject target;
    public override void OnPlay()
    {
        base.OnPlay();
        if (type.Equals(CinemaActiveObjectType.player))
            target = GameInstance.Instance.PlayerController.ControlledPawn.gameObject;
        target.SetActive(active);
    }

    public override bool OnUpdate()
    {
        return target.activeSelf == active;
    }

    public override void OnSkip()
    {
        OnPlay();
        base.OnSkip();
    }
}
