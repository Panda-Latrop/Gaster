using System.Collections;
using UnityEngine;

public class ActionCameraMovementComponent : ActionBaseComponent
{
    protected enum CinemaCameraMovementType
    {
        teleport,
        moveToPoint,
        moveByPath,
    }
    [SerializeField]
    protected CinemaCameraMovementType type;
    [SerializeField]
    protected Vector3 point;
    [SerializeField]
    protected GameObject path;

    public override void OnEnter()
    {
        base.OnEnter();
        switch (type)
        {
            case CinemaCameraMovementType.teleport:
                GameInstance.Instance.Camera.Movement.Teleport(point);
                break;
            case CinemaCameraMovementType.moveToPoint:
                GameInstance.Instance.Camera.Movement.MoveTo(point);
                break;
            case CinemaCameraMovementType.moveByPath:
                //GameInstance.Instance.Camera.Movement.MoveTo(path);
                break;
        }
    }

    public override bool OnUpdate()
    {
        switch (type)
        {
            case CinemaCameraMovementType.teleport:
                return true;
            case CinemaCameraMovementType.moveToPoint:
            case CinemaCameraMovementType.moveByPath:
                return  GameInstance.Instance.Camera.Movement.ArrivalPath;          
        }
        return false;
    }

    protected void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            switch (type)
            {
                case CinemaCameraMovementType.teleport:
                case CinemaCameraMovementType.moveToPoint:
                    Gizmos.DrawSphere(point, 0.75f);
                    break;
                case CinemaCameraMovementType.moveByPath:
                    break;
            }
        }
    }


}