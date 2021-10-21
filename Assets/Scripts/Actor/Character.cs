using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void Physics2DHitDelegate(Collision2D collision);
public class Character : Pawn
{
    [SerializeField]
    protected CapsuleCollider2D capsule;
    public bool useHitEvent;
    [SerializeField]
    protected new Rigidbody2D rigidbody;
    [SerializeField]
    protected CharacterOrientationBaseComponent characterOrientation;
    [SerializeField]
    protected CharacterMovementComponent characterMovement;


    protected Physics2DHitDelegate OnHit;


    public CapsuleCollider2D Capsule { get => capsule; }
    public Rigidbody2D Rigidbody { get => rigidbody; }
    public CharacterOrientationBaseComponent CharacterOrientation { get => characterOrientation; }
    public CharacterMovementComponent CharacterMovement { get => characterMovement; }

    protected override void Start()
    {
        base.Start();
        if (transform.localRotation.y == 1.0f || transform.localRotation.y == -1.0f)
        {
            Vector2 orientation = characterOrientation.Orientation;
            orientation.x *= -1.0f;
            characterOrientation.Orientation = orientation;
            Vector2 direction = characterMovement.Direction;
            direction.x *= -1.0f;
            characterMovement.Direction = direction;

            transform.localRotation = Quaternion.identity;
        }
    }

    public void MoveInput(Vector2 input)
    {
        characterMovement.Move(input.normalized);
    }

    public MovePathResult MoveTo(Vector3 point, bool forceChangePath = false)
    {
        return characterMovement.MoveTo(point, forceChangePath);
    }
   
    #region Hurt
    protected override void OnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        capsule.enabled = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.rotation = 0.0f;
        rigidbody.simulated = false;
        characterOrientation.enabled = false;
        characterMovement.Stop();
        characterMovement.enabled = false;

        OnHit = null;
        base.OnDeath(ds, raycastHit);
    }
    protected override void OnResurrect()
    {
        capsule.enabled = true;
        rigidbody.simulated = true;
        characterOrientation.enabled = true;
        characterMovement.enabled = true;
        base.OnResurrect();
    }
    #endregion

    #region Delegate
    public void CallOnHit(Collision2D collision)
    {
        OnHit?.Invoke(collision);
    }
    public void BindOnOnHit(Physics2DHitDelegate action)
    {
        OnHit += action;
    }
    public void UnbindOnHit(Physics2DHitDelegate action)
    {
        OnHit -= action;
    }
    public void ClearOnHit()
    {
        OnHit = null;
    }
    #endregion

    #region Save
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("useHitEvent", new JSONBool(useHitEvent));
        SaveSystem.ColliderSave(jsonObject, "physics", capsule);
        SaveSystem.RigidbodySave(jsonObject, rigidbody);
        jsonObject.Add("movement", characterMovement.Save(new JSONObject()));
        jsonObject.Add("orientation", characterOrientation.Save(new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        useHitEvent = jsonObject["useHitEvent"].AsBool;
        SaveSystem.ColliderLoad(jsonObject, "physics", capsule);
        SaveSystem.RigidbodyLoad(jsonObject, rigidbody);
        characterMovement.Load(jsonObject["movement"].AsObject);
        characterOrientation.Load(jsonObject["orientation"].AsObject);
        return jsonObject;
    }
    #endregion



    protected override void OnDestroy()
    {
        OnHit = null;
        base.OnDestroy();
    }
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (useHitEvent)
        {
            CallOnHit(collision);
        }
    }

}
