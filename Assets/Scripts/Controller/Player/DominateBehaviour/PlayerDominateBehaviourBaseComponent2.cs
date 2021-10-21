using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerDominateBehaviourBaseComponent2 : MonoBehaviour, ISaveableComponent
{
    protected bool hasCharacter;
    protected bool hasTarget;

    public bool HasCharacter => hasCharacter;
    public bool HasTarget => hasTarget;
    public abstract void SetCharacter(CharacterAttacker character);
    public abstract void SetTarget(Pawn target);
    public abstract bool GetCharacter(ref CharacterAttacker character);
    public abstract void SetTarget(Vector3 target);
    public abstract bool Execute();
    public abstract bool LateExecute();
    public abstract void Stop();
    public abstract JSONObject Save( JSONObject jsonObject);
    public abstract JSONObject Load( JSONObject jsonObject);
}