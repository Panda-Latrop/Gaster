using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrajectoryRadialComponent : WeaponTrajectoryBaseComponent
{
    public override void SetCount(int count)
    {
        this.count = count;
        if(count <= 0)
        {
            enabled = false;
        }
        else
        {
            enabled = true;
            step = 360.0f / count;
        }
    }
    public override void RandomShift()
    {
        shift = Random.Range(0.0f, 360.0f);
    }
    public override Vector3 Evaluate(int position)
    {
        Vector3 pos = (Quaternion.AngleAxis(step * fill * position + shift, Vector3.forward) * Vector3.right) * size * scale;
        return transform.position + transform.rotation * pos;
        //return Vector2.zero;
    }

    public override void OnUpdate()
    {

        shift += Time.deltaTime*360.0f * shiftSpeed;
       // if (shift >= 360.0f)
        //    shift = shift - 360.0f;
    }
}
