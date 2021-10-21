using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraActor : Actor
{
    [SerializeField]
    protected new Camera camera;
    [SerializeField]
    protected CameraMovementComponent movement;
    [SerializeField]
    protected CameraFadeComponent fade;
    [SerializeField]
    protected CameraZoomComponent zoom;
    public Camera Instance => camera;
    public CameraMovementComponent Movement => movement;
    public CameraFadeComponent Fade => fade;
    public CameraZoomComponent Zoom => zoom;

    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("movement",movement.Save(new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        movement.Load(jsonObject["movement"].AsObject);
        return jsonObject;
    }
}
