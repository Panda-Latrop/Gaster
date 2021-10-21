using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaCameraZoomComponent : CinemaBaseComponent
{
    public enum CinemaCameraZoomType
    {
        to,
        reset,
    }
    [SerializeField]
    protected CinemaCameraZoomType type;
    [SerializeField]
    protected float zoom;
    public override void OnPlay()
    {
        base.OnPlay();
        switch (type)
        {
            case CinemaCameraZoomType.to:
                GameInstance.Instance.Camera.Zoom.To(zoom);
                break;
            case CinemaCameraZoomType.reset:
                GameInstance.Instance.Camera.Zoom.Default();
                break;
            default:
                break;
        }
    }
    public override bool OnUpdate()
    {
        return !GameInstance.Instance.Camera.Zoom.enabled;
    }
    public override void OnSkip()
    {
        base.OnSkip();
        GameInstance.Instance.Camera.Zoom.Default();
    }
}
