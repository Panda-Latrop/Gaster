using System.Collections;
using UnityEngine;


public class ActionLevelChangeComponent : ActionBaseComponent
{
    [SerializeField]
    protected string toLevel;
    [SerializeField]
    protected int toEnter;
    public override void OnEnter()
    {
        base.OnEnter();
        waitForEnd = true;
        GameInstance.Instance.Camera.Fade.Out(4.0f);
        //finished = true;

    }
    public override bool OnUpdate()
    {
        if (!GameInstance.Instance.Camera.Fade.IsFading)
        {
            GameInstance.Instance.LoadScene(toLevel, toEnter);
            return true;
        }
        return false;
    }
}