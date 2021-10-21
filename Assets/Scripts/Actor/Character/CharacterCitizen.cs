using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCitizen : CharacterAttacker
{
    [SerializeField]
    protected bool useDialog = true;
    [SerializeField]
    protected CharacterDialogComponent dialog;

    public CharacterDialogComponent Dialog => dialog;

    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        if (useDialog)
            jsonObject.Add("dialog", dialog.Save(new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        if (useDialog)
            dialog.Load(jsonObject["dialog"].AsObject);
        return jsonObject;
    }
}
