using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScenarioComponent : ActionBaseComponent
{
    [SerializeField]
    protected CinemaScenarioBase scenario;
    public override void OnEnter()
    {
        base.OnEnter();
        GameInstance.Instance.Cinema.Play(scenario);
    }
    public override bool OnUpdate()
    {
        return true;
    }
}
