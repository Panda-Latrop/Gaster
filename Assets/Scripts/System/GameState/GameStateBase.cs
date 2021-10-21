using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateBase : Actor
{
    protected int totalAIInCombat;

    public int TotalAIInCombat => totalAIInCombat;
    public void AddInCombat()
    {
        totalAIInCombat++;
    }
    public void RemoveInCombat()
    {
        totalAIInCombat--;
    }

    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("totalAIInCombat", new JSONNumber(totalAIInCombat));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        totalAIInCombat = jsonObject["totalAIInCombat"].AsInt;
        return jsonObject;
    }
}
