using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBaseComponent : MonoBehaviour, ISaveableComponent
{
    public abstract JSONObject Save( JSONObject jsonObject);
    public abstract JSONObject Load( JSONObject jsonObject);
}
