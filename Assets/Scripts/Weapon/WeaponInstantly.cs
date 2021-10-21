using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInstantly : WeaponBase
{
    [SerializeField]
    protected float overlapRadius = 1.0f;
    protected Collider2D[] hits = new Collider2D[5];
    protected Vector3 target;

    public override void SetTarget(Vector3 target)
    {
        this.target = target;
    }
    protected override void PlayMuzzleFlash()
    {
        if (hasMuzzle)
        {
            muzzleFlash.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.time = 0.0f;
            muzzleFlash.transform.position = target;
            muzzleFlash.Play(true);
        }
    }
    public override ShootState Shoot(Vector3 position, Vector3 direction)
    {
        if (CanShoot())
        {
            //enabled = true;
            CreateProjectile(target, direction);
            PlayMuzzleFlash();
            PlayAudio();
            shootState = ShootState.initiated;
            return shootState;
        }
        shootState = ShootState.unready;
        return shootState;
    }
    protected override RaycastHit2D CreateProjectile(Vector3 position, Vector3 direction)
    {
        int count = Physics2D.OverlapCircleNonAlloc(position, overlapRadius, hits, LayerMask.GetMask("Pawn"));
        for (int i = 0; i < count; i++)
        {
            IHealth health = hits[i].GetComponent<IHealth>();
            if (health != null && !health.Team.Equals(owner.Health.Team))
            {
                health.Hurt(new DamageStruct(owner.gameObject, owner.Health.Team, damage * damageMultiply, (hits[i].transform.position - position).normalized, power), default);
            }
        }
        return default;
    }
    protected void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, overlapRadius);
        }
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        JSONArray targetJArray = new JSONArray();
        {
            targetJArray.Add(new JSONNumber(target.x));
            targetJArray.Add(new JSONNumber(target.y));
            targetJArray.Add(new JSONNumber(target.z));
        }
        jsonObject.Add("target", targetJArray);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        JSONArray targetJArray = jsonObject["target"].AsArray;
        target.Set(targetJArray[0].AsFloat, targetJArray[1].AsFloat, targetJArray[2].AsFloat); 
        return jsonObject;
    }
}
