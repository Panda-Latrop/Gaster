using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaSpeechComponent : CinemaBaseComponent
{
    [SerializeField]
    protected CinemaDialogComponent.CinemaDialogType type = CinemaDialogComponent.CinemaDialogType.next;
    [SerializeField]
    protected string file;
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
            case CinemaDialogComponent.CinemaDialogType.prepare:
                GameInstance.Instance.Dialog.Prepare(file);
                break;
            case CinemaDialogComponent.CinemaDialogType.next:
                    nextSkip = Time.time + timeToSkip;
                GameInstance.Instance.Dialog.Next(false);
                break;
            case CinemaDialogComponent.CinemaDialogType.hide:
                GameInstance.Instance.Dialog.Hide();
                break;
        }
    }

    public override bool OnUpdate()
    {
        switch (type)
        {
            case CinemaDialogComponent.CinemaDialogType.prepare:
                return true;
            case CinemaDialogComponent.CinemaDialogType.next:
                if (GameInstance.Instance.Dialog.Skipped)
                {
                    GameInstance.Instance.Dialog.Hide();
                    return true;
                }
                if (Time.time >= nextSkip)
                {
                    if (hideAfterSkip)
                        GameInstance.Instance.Dialog.Hide();
                    return true;
                }
                break;
            case CinemaDialogComponent.CinemaDialogType.hide:
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