using System.Collections;
using UnityEngine;

public class CinemaCameraMovementComponent : CinemaBaseComponent
{
    public enum CinemaCameraMovementType
    {
        teleport,
        moveToPoint,
        moveByPath,
    }
    [SerializeField]
    protected CinemaCameraMovementType type;
    [SerializeField]
    protected float speedMultiply = 1.0f;
    [SerializeField]
    protected Vector3 point;
    [SerializeField]
    protected GameObject path;

    public override void OnPlay()
    {
        base.OnPlay();
        switch (type)
        {
            case CinemaCameraMovementType.teleport:
                GameInstance.Instance.Camera.Movement.Teleport(point);
                break;
            case CinemaCameraMovementType.moveToPoint:
                GameInstance.Instance.Camera.Movement.speedMultiply = speedMultiply;
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
                {
                    if (GameInstance.Instance.Camera.Movement.ArrivalPath)
                    {
                        GameInstance.Instance.Camera.Movement.speedMultiply = 1.0f;
                        return true;
                    }
                }
                break;
        }
        return false;
    }
    public override void OnSkip()
    {
        switch (type)
        {
            case CinemaCameraMovementType.teleport:
            case CinemaCameraMovementType.moveToPoint:
                GameInstance.Instance.Camera.Movement.Teleport(point);
                break;
            case CinemaCameraMovementType.moveByPath:
                //GameInstance.Instance.Camera.Movement.MoveTo(path);
                break;
        }
        GameInstance.Instance.Camera.Movement.speedMultiply = 1.0f;
        base.OnSkip();
    }
    protected void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying && !type.Equals(CinemaCameraMovementType.moveByPath))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(point, 0.75f);
        }
    }
}