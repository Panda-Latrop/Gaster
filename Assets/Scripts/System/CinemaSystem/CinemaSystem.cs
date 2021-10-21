using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaSystem : MonoBehaviour
{
    protected bool hasScenario;
    protected CinemaScenarioBase scenario;
    public bool HasScenario => hasScenario;
    public bool OverrideInput => hasScenario && scenario.OverrideInput;


    public void Play(CinemaScenarioBase scenario)
    {
        Skip();
        if (!hasScenario)
        {
            enabled = hasScenario = true;           
            this.scenario = scenario;
            this.scenario.Play();
        }
    }
    public void Skip()
    {
        if (hasScenario)
        {
            scenario.Skip();
            enabled = hasScenario = false;
        }
    }
    public void Stop()
    {
        if (hasScenario)
        {
            enabled = hasScenario = false;
        }
    }
    protected void Update()
    {
        if (hasScenario)
        {
            scenario.OnUpdate();
        }
    }
    protected void LateUpdate()
    {
        if (hasScenario)
        {
            if (!scenario.OnLateUpdate())
            {
                Stop();
            }
        }
    }


}
