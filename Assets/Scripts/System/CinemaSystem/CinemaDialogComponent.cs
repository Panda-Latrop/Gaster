using System.Collections;
using UnityEngine;


public class CinemaDialogComponent : CinemaBaseComponent
{

    public enum CinemaDialogType
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
    [SerializeField]
    protected bool hideAfterSkip;
    protected float nextSkip;

    public override void OnPlay()
    {
        base.OnPlay();
        switch (type)
        {
            case CinemaDialogType.prepare:
                GameInstance.Instance.Dialog.Prepare(file);
                break;
            case CinemaDialogType.next:               
                if (useTime)
                    nextSkip = Time.time + timeToSkip;
                GameInstance.Instance.Dialog.Next(true);
                break;
            case CinemaDialogType.hide:
                GameInstance.Instance.Dialog.Hide();
                break;
        }
    }

    public override bool OnUpdate()
    {
        switch (type)
        {
            case CinemaDialogType.prepare:
                return true;
            case CinemaDialogType.next:
                if (GameInstance.Instance.Dialog.Skipped)
                {
                    GameInstance.Instance.Dialog.Hide();
                    return true;
                }
                if (useTime && Time.time >= nextSkip)
                {
                    if (hideAfterSkip)
                    GameInstance.Instance.Dialog.Hide();
                    return true;
                }
                break;
            case CinemaDialogType.hide:
                return true;
        }
        return false;
    }
    public override void OnSkip()
    {
        GameInstance.Instance.Dialog.End();
        base.OnSkip();
    }
}
