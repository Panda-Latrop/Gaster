using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPressPlate : TriggerPress
{
    [SerializeField]
    protected Sprite close, open;
    [SerializeField]
    protected new SpriteRenderer renderer;

    public override void Open()
    {
        base.Open();
        renderer.sprite = open;
    }
    public override void Close()
    {
        base.Close();
        renderer.sprite = close;
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save(jsonObject);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        if(isClosed)
            renderer.sprite = close;
        else
            renderer.sprite = open;
        return jsonObject;
    }
}
