using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBoss : Pawn
{
    [SerializeField]
    protected new Rigidbody2D rigidbody;
    [SerializeField]
    protected OrientationComponent orientation;
    [SerializeField]
    protected CharacterMovementComponent characterMovement;

    public Rigidbody2D Rigidbody { get => rigidbody; }
    public OrientationComponent Orientation { get => orientation; }
    public CharacterMovementComponent CharacterMovement { get => characterMovement; }

    protected override void Start()
    {
        base.Start();
        if (transform.localRotation.y == 1.0f || transform.localRotation.y == -1.0f)
        {
            Vector2 orientation = this.orientation.Direction;
            orientation.x *= -1.0f;
            this.orientation.Direction = orientation;
            Vector2 direction = characterMovement.Direction;
            direction.x *= -1.0f;
            characterMovement.Direction = direction;

            transform.localRotation = Quaternion.identity;
        }
    }
    #region Hurt
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        //capsule.enabled = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.rotation = 0.0f;
        rigidbody.simulated = false;
        orientation.enabled = false;
        characterMovement.Stop();
        characterMovement.enabled = false;
        base.OnDeath(ds, raycastHit);
    }
    protected override void OnResurrect()
    {
        //capsule.enabled = true;
        rigidbody.simulated = true;
        orientation.enabled = true;
        characterMovement.enabled = true;
        base.OnResurrect();
    }
    #endregion
    #region Save
    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        SaveSystem.RigidbodySave(jsonObject, rigidbody);
        jsonObject.Add("movement", characterMovement.Save(new JSONObject()));
        jsonObject.Add("orientation", orientation.Save(new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        //SaveSystem.ColliderLoad(jsonObject, "physics", capsule);
        SaveSystem.RigidbodyLoad(jsonObject, rigidbody);
        characterMovement.Load(jsonObject["movement"].AsObject);
        orientation.Load(jsonObject["orientation"].AsObject);
        return jsonObject;
    }
    #endregion
}