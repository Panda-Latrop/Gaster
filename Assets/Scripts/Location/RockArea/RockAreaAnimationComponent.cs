using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockAreaAnimationComponent : MonoBehaviour
{
    [SerializeField]
    protected Transform rock;
    protected float angle;
    [SerializeField]
    protected float angular = 60.0f;
    public bool OnLateUpdate()
    {
        angle -= angular * Time.deltaTime;
        rock.localRotation = Quaternion.AngleAxis(angle,Vector3.forward);
        if (angle >= 180.0f)
        {
            angle -= 360.0f;
        }
        if (angle <= -180.0f)
        {
            angle += 360.0f;
        }
        return true;
    }
}
