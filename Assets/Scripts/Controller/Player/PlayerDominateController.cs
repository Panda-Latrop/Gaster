using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDominateController : PlayerController
{

    [Header("Targets")]
    protected List<CharacterAttacker> targets = new List<CharacterAttacker>();
    [SerializeField]
    protected PlayerDominateBehaviourHolder behaviourHolder;

    public PlayerDominateBehaviourHolder BehaviourHolder => behaviourHolder;
    public int TargetCount => targets.Count;
    public void RemoveTarget(CharacterAttacker character)
    {
        targets.Remove(character);
    }
    public override void Possess(Pawn pawn)
    {
        base.Possess(pawn);
        player = controlledPawn as CharacterPlayer;
       // player.WeaponHolderComponent.Dominator.ClearOnDominate();
        player.WeaponHolderComponent.Dominator.BindOnDominate(OnDominate);
    }
    public override void Unpossess()
    {
        player = null;
        player.WeaponHolderComponent.Dominator.UnbindOnDominate(OnDominate);
        base.Unpossess();
    }
    public bool ClosestCharacter(ref CharacterAttacker characterAttacker, Vector3 position)
    {
        float minM = float.MaxValue;
        float minTmp;
        bool hasCharacter = false;
        for (int i = 0; i < targets.Count; i++)
        {
            minTmp = (targets[i].transform.position - position).sqrMagnitude;
            if (minM > minTmp)
            {
                hasCharacter = true;
                minM = minTmp;
                characterAttacker = targets[i];
            }
        }
        return hasCharacter;
    }
    public bool ClosestCharacter(ref CharacterAttacker characterAttacker, CharacterAttacker ignore, Vector3 position)
    {
        float minM = float.MaxValue;
        float minTmp;
        bool hasCharacter = false;
        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i].Equals(ignore)) {
                minTmp = (targets[i].transform.position - position).sqrMagnitude;
                if (minM > minTmp)
                {
                    hasCharacter = true;
                    minM = minTmp;
                    characterAttacker = targets[i];
                }
            }
        }
        return hasCharacter;
    }
    protected bool ChagneTarget(ref CharacterAttacker target)
    {
        return ChagneTarget(ref target, controlledPawn.transform.position);
    }
    protected bool ChagneTarget(ref CharacterAttacker target, Vector3 position)
    {
        ClosestCharacter(ref target, position);
        while (!target.Health.IsAlive)
        {
            targets.Remove(target);
            if (!ClosestCharacter(ref target, position))
            {
                return false;
            }
        }
        target.Controller.Unpossess();
        target.OnPossess(this);
        return true;
    }

    protected override void GameControll()
    {
        base.GameControll();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            behaviourHolder.ChangeSlotPrev();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            behaviourHolder.ChangeSlotNext();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            behaviourHolder.Execute(0);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            behaviourHolder.Execute(1);
        }


    }

    protected override void Update()
    {
        base.Update();
        behaviourHolder.OnUpdate();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i].Health.IsAlive)
            {
                targets[i].SaveTag = "";
                targets.RemoveAt(i);
                i--;
            }
        }
        behaviourHolder.OnLateUpdate();
        /*if (healBehaviour.HasCharacter && healBehaviour.HasTarget)
            healBehaviour.LateExecute();
        if (bombBehaviour.HasCharacter && bombBehaviour.HasTarget)
            bombBehaviour.LateExecute();
        if (lightningBehaviour.HasCharacter && lightningBehaviour.HasTarget)
            lightningBehaviour.LateExecute();*/
    }


    protected void OnDominate(Pawn dominated)
    {

        CharacterAttacker character = dominated as CharacterAttacker;
        character.Health.Team = player.Health.Team;
        //Debug.Log("OnDominate " + character.name);
        character.SaveTag = "next";
        character.Controller.SaveTag = "next";
        targets.Add(character);
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        JSONArray targetsJArray = new JSONArray();
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].Health.IsAlive)
                {
                    targetsJArray.Add(SaveSystem.ComponentReferenceSave(new JSONObject(), "target", targets[i]));
                }
            }
        }
        jsonObject.Add("targets", targetsJArray);
        jsonObject.Add("behaviourHolder", behaviourHolder.Save(new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        JSONArray targetsJArray = jsonObject["targets"].AsArray;
        {
            for (int i = 0; i < targetsJArray.Count; i++)
            {
                CharacterAttacker character = default;
                if(SaveSystem.ComponentReferenceLoad(targetsJArray[i].AsObject, "target", ref character))
                    targets.Add(character);
            }
        }
        behaviourHolder.Load(jsonObject["behaviourHolder"].AsObject);
        return jsonObject;
    }
    protected override void OnDestroy()
    {
        player.WeaponHolderComponent.Dominator.UnbindOnDominate(OnDominate);
        base.OnDestroy();
    }
    public override void SetStart(Vector3 position, Quaternion rotation)
    {
        base.SetStart(position, rotation);
        position += (Vector3.down + Vector3.left)*2.0f;
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].transform.position = position;
            targets[i].transform.rotation = rotation;
        }
    }
}
