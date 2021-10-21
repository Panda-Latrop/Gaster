using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaMovementComponent : CinemaBaseComponent
{
    protected enum CinemaMovementType
    {
        teleport,
        moveToPoint,
        moveByPath,
    }
    [SerializeField]
    protected CharacterMovementComponent movement;
    [SerializeField]
    protected CinemaMovementType type;
    [SerializeField]
    protected bool simple;
    [SerializeField]
    protected Vector3 point;
    [SerializeField]
    protected GameObject path;

    public override void OnPlay()
    {
        base.OnPlay();
        switch (type)
        {
            case CinemaMovementType.teleport:
                movement.Teleport(point);
                break;
            case CinemaMovementType.moveToPoint:
                if (simple)
                {
                    List<Vector3> list = new List<Vector3>();
                    list.Add(point);
                    movement.MoveTo(list);
                }
                else
                    movement.MoveTo(point, true);
                break;
            case CinemaMovementType.moveByPath:
                //character.CharacterMovement.MoveTo(path);
                break;
        }
    }

    public override bool OnUpdate()
    {
        switch (type)
        {
            case CinemaMovementType.teleport:
                return true;
            case CinemaMovementType.moveToPoint:
            case CinemaMovementType.moveByPath:
                return movement.ArrivalPath;
        }
        return false;
    }
    public override void OnSkip()
    {
        switch (type)
        {
            case CinemaMovementType.teleport:
            case CinemaMovementType.moveToPoint:
                movement.Teleport(point);
                break;
            case CinemaMovementType.moveByPath:
                //character.CharacterMovement.MoveTo(path);
                break;
        }
        base.OnSkip();
    }
    protected void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying && !type.Equals(CinemaMovementType.moveByPath))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(point, 0.75f);
        }
    }
}
