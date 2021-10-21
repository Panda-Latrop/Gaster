using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPress : TriggerCollision
{
    [SerializeField]
    protected bool openOnExit = false;

    protected override void OnEnter(Collider2D other)
    {
        if(enabled && currentExecutor != 0)
            Stop();
        currentExecutor = 0;
        base.OnEnter(other);
    }
    protected override void OnExit(Collider2D other)
    {

        if (isClosed && openOnExit)
        {
            if (enabled && currentExecutor != 1)
                Stop();
            currentExecutor = 1;
            bool execute = false;
            switch (type)
            {
                case TriggerCollisionType.player:
                    execute = other.gameObject.Equals(GameInstance.Instance.PlayerController.ControlledPawn.gameObject);
                    break;
                case TriggerCollisionType.actor:
                    execute = other.gameObject.Equals(actor);
                    break;
                case TriggerCollisionType.tag:
                    execute = other.tag.Equals(targetTag);
                    break;
                case TriggerCollisionType.anything:
                    execute = true;
                    break;
            }
            if (execute)
            {
                isClosed = false;
                Execute();
                Open();
            }
        }
    }
}
