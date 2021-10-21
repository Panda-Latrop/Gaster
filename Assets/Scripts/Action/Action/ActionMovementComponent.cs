using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMovementComponent : ActionBaseComponent
{
    protected enum CinemaMovementType
    {
        teleport,
        moveToPoint,
        moveByPath,
    }
    [SerializeField]
    protected Character character;
    [SerializeField]
    protected CinemaMovementType type;
    [SerializeField]
    protected Vector3 point;
    [SerializeField]
    protected GameObject path;

    public override void OnEnter()
    {
        base.OnEnter();
        switch (type)
        {
            case CinemaMovementType.teleport:
                character.CharacterMovement.Teleport(point);
                break;
            case CinemaMovementType.moveToPoint:
                character.CharacterMovement.MoveTo(point, true);
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
                return   character.CharacterMovement.ArrivalPath;
        }
        return false;
    }
    /*public override JSONObject Save( JSONObject jsonObject)
    {
base.Save( jsonObject);
        JSONObject actionJObject = jsonObject[GetType().ToString().ToLower()].AsObject;
        //actionJObject.Add("type", new JSONNumber((int)type));
        //actionJObject.Add("nextSkip", new JSONNumber(nextSkip));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        JSONObject actionJObject = jsonObject[GetType().ToString().ToLower()].AsObject;
        //type = (CinemaMovementType)actionJObject["type"].AsInt;
        //nextSkip = actionJObject["nextSkip"].AsFloat;
        return jsonObject;
    }*/
}
