using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputRotationComponent : MonoBehaviour
{
    [SerializeField]
    protected CharacterOrientationBaseComponent characterOrientation;
    [SerializeField]
    protected Transform arms;

    public Transform Arms => arms;

    public void SetRotation(Vector3 direction)
    {
        characterOrientation.Orientation = direction;
        direction.Set(-direction.y, direction.x, 0);
        Quaternion q = Quaternion.LookRotation(Vector3.forward, direction);
        arms.localRotation =q;
        //Debug.Log(q.eulerAngles.ToString());
    }
}
