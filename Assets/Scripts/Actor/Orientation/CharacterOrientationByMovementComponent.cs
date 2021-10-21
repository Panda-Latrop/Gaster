using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterOrientationByMovementComponent : CharacterOrientationBaseComponent
{
    [SerializeField]
    protected CharacterMovementComponent characterMovement;
    public CharacterMovementComponent CharacterMovement => characterMovement;

    private void LateUpdate()
    {
        orientation = characterMovement.Direction;
    }
}
