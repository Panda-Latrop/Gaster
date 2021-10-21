using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerRandomMove : AIControllerBase
{
    public float time = 1.5f;
    protected float nextTime;

    protected Character character;
    protected CharacterMovementComponent characterMovement;

    public override void Possess(Pawn pawn)
    {
        base.Possess(pawn);
        character = pawn as Character;
        characterMovement = character.CharacterMovement;
    }

    protected void Update()
    {
        if(Time.time >= nextTime)
        {
            nextTime = Time.time + time;
            NavigationGrid grid = GameInstance.Instance.Navigation.grid;
            int r = Random.Range(0, grid.Length);
            character.MoveTo(grid.GetNode(r).position,true);
        }
    }

    #region Save
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        SaveSystem.TimerSave(jsonObject, "", nextTime);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        SaveSystem.TimerLoad(jsonObject, "",ref nextTime);
        return jsonObject;
    }
    #endregion
}
