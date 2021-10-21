using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSetGlobalComponent : DeathComponent
{
    [System.Serializable]
    public struct SetGlobalStruct
    {
        public GameInstance.GlobalStruct global;
        public int action;
    }
    [SerializeField]
    protected SetGlobalStruct[] globals;
    public override void Execute(DamageStruct ds, RaycastHit2D raycastHit)
    {
        if (enabled)
        {
            base.Execute(ds, raycastHit);
            for (int i = 0; i < globals.Length; i++)
            {
                if(globals[i].action == 0)
                GameInstance.Instance.GlobalSet(globals[i].global);
                else
                    GameInstance.Instance.GlobalAdd(globals[i].global);
            }
        }
    }
}
    