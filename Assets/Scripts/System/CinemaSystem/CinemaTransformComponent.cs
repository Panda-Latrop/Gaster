using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaTransformComponent : CinemaBaseComponent
{
    public enum CinemaTransformActionType
    {
        ignore,
        reset,
        set,
        offset,
    }
    public enum CinemaTransformType
    {
        position,
        rotation,
        scale,
    }

    [SerializeField]
    protected new Transform transform;
    [SerializeField]
    protected CinemaTransformType type;
    [SerializeField]
    protected CinemaTransformActionType actionType;
    [SerializeField]
    protected Vector3 vector;

    public override void OnPlay()
    {
        base.OnPlay();
        switch (type)
        {   
            case CinemaTransformType.position:
                switch (actionType)
                {
                    case CinemaTransformActionType.ignore:
                        break;
                    case CinemaTransformActionType.reset:
                        transform.position = Vector3.zero;
                        break;
                    case CinemaTransformActionType.set:
                        transform.position = vector;
                        break;
                    case CinemaTransformActionType.offset:
                        transform.position = transform.position + vector;
                        break;
                    default:
                        break;
                }
                break;
            case CinemaTransformType.rotation:
                switch (actionType)
                {
                    case CinemaTransformActionType.ignore:
                        break;
                    case CinemaTransformActionType.reset:
                        transform.rotation = Quaternion.identity;
                        break;
                    case CinemaTransformActionType.set:
                        transform.rotation = Quaternion.Euler(vector);
                        break;
                    case CinemaTransformActionType.offset:
                        transform.rotation = transform.rotation * Quaternion.Euler(vector);
                        break;
                    default:
                        break;
                }
                break;
            case CinemaTransformType.scale:
                switch (actionType)
                {
                    case CinemaTransformActionType.ignore:
                        break;
                    case CinemaTransformActionType.reset:
                        transform.localScale = Vector3.zero;
                        break;
                    case CinemaTransformActionType.set:
                        transform.localScale = vector;
                        break;
                    case CinemaTransformActionType.offset:
                        transform.localScale = transform.localScale + vector;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
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
