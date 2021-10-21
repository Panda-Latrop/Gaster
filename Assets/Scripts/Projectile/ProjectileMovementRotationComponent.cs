using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovementRotationComponent : ProjectileMovementComponent
{
    [SerializeField]
    protected Vector3 direction;
    [SerializeField]
    protected float maxRotationSpeed = 180.0f;
    public override void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0)
        {
            hasInput = true;
            this.direction = direction;
        }
        else
        {
            hasInput = false;
        }          
    }
    protected override void FixedUpdate()
    {
        if (hasInput)
        {           
            rigidbody.SetRotation(Quaternion.RotateTowards(transform.rotation,
                                                            Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward), 
                                                            maxRotationSpeed * Time.deltaTime));
            rigidbody.velocity = transform.right* speed;
            hasInput = false;
        }
        else
        {
            rigidbody.SetRotation(Quaternion.RotateTowards(transform.rotation,
                                                            Quaternion.LookRotation(Vector3.forward, -transform.right),
                                                            maxRotationSpeed * Time.deltaTime));
            rigidbody.velocity = transform.right * speed;
        }

    }

    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        JSONArray directionJArray = new JSONArray();
        {
            directionJArray.Add(new JSONNumber(direction.x));
            directionJArray.Add(new JSONNumber(direction.y));
        }
        jsonObject.Add("direction", directionJArray);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        JSONArray directionJArray = jsonObject["direction"].AsArray;
        direction.Set(directionJArray[0].AsFloat, directionJArray[1].AsFloat,0.0f);
        return jsonObject;
    }

}
