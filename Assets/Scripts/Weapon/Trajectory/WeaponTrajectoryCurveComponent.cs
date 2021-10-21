using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrajectoryCurveComponent : WeaponTrajectoryBaseComponent
{

    [SerializeField]
    protected AnimationCurve xCurve, yCurve;



    public override void SetCount(int count)
    {
        this.count = count;
        if (count <= 0)
        {
            enabled = false;
        }
        else
        {
            enabled = true;
            step = 1.0f / count;           
        }
    }
    public override Vector3 Evaluate(int position)
    {
        float time = step * fill * position + shift;
        return transform.position + transform.rotation*(new Vector3(xCurve.Evaluate(time), yCurve.Evaluate(time),0.0f) * size * scale);         
    }
    public override void OnUpdate()
    {
        shift = shift + Time.deltaTime * shiftSpeed;
    }
}
