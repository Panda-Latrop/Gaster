using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDominateTeleportBehaviourComponent : PlayerDominateBehaviourBaseComponent
{


    public override bool Ready()
    {
        return controller.TargetCount >= 1 && !hasCharacter && !hasTarget;
    }

    public override void Execute(int action)
    {
        if (action == 0 && Ready())
        {
            CharacterAttacker character = default;
            if (controller.ClosestCharacter(ref character, controller.GetMousePosition()))
            {
                controller.Player.CharacterMovement.Teleport(character.transform.position);
                character.Health.Kill();

            }
        }
    }
    public override bool OnUpdate()
    {
        return true;
    }
    public override bool OnLateUpdate()
    {
        return true;
    }

    public override void Stop()
    {
        return;
    }
}
