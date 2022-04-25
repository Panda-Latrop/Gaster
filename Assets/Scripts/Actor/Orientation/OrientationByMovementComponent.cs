using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationByMovementComponent : OrientationComponent
{
    [SerializeField]
    protected CharacterMovementComponent characterMovement;
    public CharacterMovementComponent CharacterMovement => characterMovement;

    private void LateUpdate()
    {
        direction = characterMovement.Direction;
    }
}
