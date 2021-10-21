using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAddToPlayerBehaviourComponent : ActionBaseComponent
{
    [SerializeField]
    protected PlayerDominateBehaviourBaseComponent behaviour;
    public override void OnEnter()
    {
        base.OnEnter();
        (GameInstance.Instance.PlayerController as PlayerDominateController).BehaviourHolder.AddBehaviour(behaviour, true);
    }
    public override bool OnUpdate()
    {
        return true;
    }
}
