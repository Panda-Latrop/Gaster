using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScenario : TriggerStart
{
    [SerializeField]
    protected CinemaScenarioBase scenario;
    protected override void Start()
    {
        scenario.BindOnEnd(Execute);
        enabled = false;
    }
}
