using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovementComponent : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected new Rigidbody2D rigidbody;
    [SerializeField]
    protected float speed;
    protected bool hasInput;
    protected Vector2 velocity;
    protected float angle;
    public Rigidbody2D Rigidbody { get => rigidbody; }
    public float Speed { get => speed; set => speed = value; }

    public virtual void SetPosition(Vector2 position)
    {
        rigidbody.MovePosition(position);
    }
    public virtual void Move(Vector2 direction)
    {
        velocity = direction * speed;
        angle = Mathf.Atan2(direction.y, direction.x)*Mathf.Rad2Deg;
        hasInput = true;
    }
    public void Stop()
    {
        rigidbody.velocity = Vector2.zero;
        hasInput = false;
    }

    protected virtual void FixedUpdate()
    {
        if (hasInput)
        {
            rigidbody.velocity = velocity;
            rigidbody.SetRotation(angle);
            hasInput = false;
        }
    }

    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("enabled", enabled);
        jsonObject.Add("speed", new JSONNumber(speed));
        jsonObject.Add("hasInput", new JSONBool(hasInput));
        JSONArray velocityJArray = new JSONArray();
        {
            velocityJArray.Add(new JSONNumber(velocity.x));
            velocityJArray.Add(new JSONNumber(velocity.y));
        }
        jsonObject.Add("velocity", velocityJArray);
        jsonObject.Add("angle", new JSONNumber(angle));
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        enabled = jsonObject["enabled"].AsBool;
        speed = jsonObject["speed"].AsFloat;       
        hasInput = jsonObject["hasInput"].AsBool;
        JSONArray velocityJArray = jsonObject["velocity"].AsArray;
        velocity.Set(velocityJArray[0].AsFloat, velocityJArray[1].AsFloat);
        angle = jsonObject["angle"].AsFloat;
        return jsonObject;
    }

}
