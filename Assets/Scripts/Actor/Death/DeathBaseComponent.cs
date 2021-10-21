using System.Collections;
using UnityEngine;


public abstract class DeathBaseComponent : MonoBehaviour
{
    public abstract void Execute(DamageStruct ds, RaycastHit2D raycastHit);
}
