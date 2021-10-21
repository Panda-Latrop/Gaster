using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDialogComponent : ActionBaseComponent
{
    protected enum CinemaDialogType
    {
        prepare,
        next,
        hide,
    }
    [SerializeField]
    protected CinemaDialogType type = CinemaDialogType.next;
    [SerializeField]
    protected string file;
    [SerializeField]
    protected bool useTime;
    [SerializeField]
    protected float timeToSkip;
    protected float nextSkip;

    public override void OnEnter()
    {
        base.OnEnter();
      /*  switch (type)
        {
            case CinemaDialogType.prepare:
                GameInstance.Instance.Dialog.Prepare(file);
                break;
            case CinemaDialogType.next:
                if (useTime)
                    nextSkip = Time.time + timeToSkip;
                GameInstance.Instance.Dialog.Next(true,true);
                break;
            case CinemaDialogType.hide:
                GameInstance.Instance.Dialog.Hide();
                break;
        }*/
    }

    public override bool OnUpdate()
    {
        /* switch (type)
         {
             case CinemaDialogType.prepare:
                 return   true;
             case CinemaDialogType.next:

                 if (GameInstance.Instance.Dialog.Skipped)
                 {
                     return   true;
                 }
                 if(useTime && Time.time >= nextSkip)
                 {
                     GameInstance.Instance.Dialog.Skip();
                     return   true;
                 }
                 break;
             case CinemaDialogType.hide:
                 return   true;
         }
         return   false;*/
        return true;
    }

    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        SaveSystem.TimerSave(jsonObject, "skip", nextSkip);
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        SaveSystem.TimerLoad(jsonObject, "skip", ref nextSkip);
        return jsonObject;
    }
}
