using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJ : MonoBehaviour
{
    [ContextMenu("Call")]
    public void Call()
    {
        string s = "1";
        B b = new B();
        Debug.Log(b.Call(ref s));
    }
}

public class A
{
    public virtual string Call(ref string s)
    {
        s += this.GetType().Name;
        return s;
    }
}
public class B:A
{
    public override string Call(ref string s)
    {
        base.Call(ref s);
        s += " B";
        return s;
    }
}