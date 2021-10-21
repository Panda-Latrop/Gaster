using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCollision : TriggerExecutor
{
    protected enum TriggerCollisionType
    {
        player,
        actor,
        tag,
        anything,

    }
    [SerializeField]
    protected Collider2D detector;
    [SerializeField]
    protected TriggerCollisionType type;
    [SerializeField]
    protected GameObject actor;
    [SerializeField]
    protected string targetTag;

#if UNITY_EDITOR
    public Color gizmosColor = Color.green;
#endif

    protected virtual void OnEnter(Collider2D other)
    {
        if (!isClosed)
        {
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
                Execute();
            }
        }
    }
    protected virtual void OnExit(Collider2D other)
    {
        return;
    }
    protected void OnTriggerEnter2D(Collider2D other)
    {
        OnEnter(other);
    }
    protected void OnTriggerExit2D(Collider2D other)
    {
        OnExit(other);
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        SaveSystem.ColliderSave(jsonObject, "main", detector);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        SaveSystem.ColliderLoad(jsonObject, "main", detector);        
        return jsonObject;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
#endif
}
