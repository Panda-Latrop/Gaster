using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMelee : WeaponBase
{ 
    [SerializeField]
    protected float timeToSwing = 1.0f;
    [SerializeField]
    protected Vector2 hurtBoxSize;
    [SerializeField]
    protected float hurtBoxLenth;
    protected RaycastHit2D[] raycastHits = new RaycastHit2D[1];
    protected float nextSwing;

    public override void SetFire(bool fire)
    {

        base.SetFire(fire);
        if (!fire)
        {
            shootState = ShootState.ended;
        }

    }
    public override ShootState Shoot(Vector3 position, Vector3 direction)
    {


        switch (shootState)
        {
            case ShootState.initiated:          
            case ShootState.process:
                shootState = ShootState.process;
                if (ReadyShoot())
                {                  
                    CreateProjectile(position, direction);
                    PlayMuzzleFlash();
                    PlayAudio();
                    shootState = ShootState.ended;               
                }
                break;
            case ShootState.unready:
            case ShootState.ended:
                if (CanShoot())
                {
                    //enabled = true;
                    nextSwing = Time.time + timeToSwing;
                    shootState = ShootState.initiated;
                    break;
                }
                shootState = ShootState.unready;
                break;
            default:
                break;
        }
        return shootState;
    }

    protected bool ReadyShoot()
    {
        return isFire && shootState.Equals(ShootState.process) && Time.time >= nextSwing;   
    }

    protected override RaycastHit2D CreateProjectile(Vector3 position, Vector3 direction)
    {
        int count = Physics2D.BoxCastNonAlloc(position, hurtBoxSize, 0.0f, direction, raycastHits, hurtBoxLenth, LayerMask.GetMask("Pawn"));
        if (count > 0)
        {
            //Debug.Log("Hit");
            IHealth health = raycastHits[0].collider.GetComponent<IHealth>();
            if(health != null && !health.Team.Equals(owner.Health.Team))
            {
                //Debug.Log(raycastHits[0].collider.name);
                health.Hurt(new DamageStruct(owner.gameObject, owner.Health.Team, damage * damageMultiply, direction, power), raycastHits[0]);
                return raycastHits[0];
            }
            return default;
        }
        //Debug.Log("Miss");
        return default;
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        SaveSystem.TimerSave(jsonObject, "swing", nextSwing);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load(jsonObject);
        SaveSystem.TimerLoad(jsonObject, "swing",ref nextSwing);        
        return jsonObject;
    }
    protected void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Vector3 offset = Vector3.right * hurtBoxLenth;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, hurtBoxSize);
            Gizmos.DrawWireCube(transform.position + offset, hurtBoxSize);
            Gizmos.DrawLine(transform.position + Vector3.up * hurtBoxSize.y / 2.0f,
                transform.position + offset + Vector3.up * hurtBoxSize.y / 2.0f);
            Gizmos.DrawLine(transform.position - Vector3.up * hurtBoxSize.y / 2.0f,
                transform.position + offset - Vector3.up * hurtBoxSize.y / 2.0f);
            Gizmos.DrawLine(transform.position + Vector3.right * hurtBoxSize.x / 2.0f,
                transform.position + offset + Vector3.right * hurtBoxSize.x / 2.0f );
            Gizmos.DrawLine(transform.position - Vector3.right * hurtBoxSize.x / 2.0f,
                transform.position + offset - Vector3.right * hurtBoxSize.x / 2.0f);
        }
    }
}
