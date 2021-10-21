using System;
using System.Collections;
using UnityEngine;

public class CinemaBaseComponent : MonoBehaviour, IComparable<CinemaBaseComponent>
{
    public bool waitForEnd;
    public float start;   
    protected bool finished;

    public bool Finished => finished;
    public virtual void OnPlay()
    {
        finished = false;
        return;
    }
    public virtual bool OnUpdate()
    {
        return true;
    }
    public virtual void OnEnd()
    {
        finished = true;
        return;
    }
    public virtual void OnSkip()
    {
        finished = true;
        return;
    }
    public int CompareTo(CinemaBaseComponent obj)
    {
        return start.CompareTo(obj.start);
    }
}