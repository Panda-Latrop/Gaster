using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShooterPrepare : WeaponShooter
{
    [SerializeField]
    protected float timeToPrepare = 1.0f;
    protected float nextPrepare;


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
                    enabled = true;
                    CreateProjectile(position, direction);
                    IncreaseSpread();
                    PlayMuzzleFlash();
                    PlayAudio();
                    shootState = ShootState.ended;
                }
                break;
            case ShootState.unready:
            case ShootState.ended:
                if (CanShoot())
                {
                    nextPrepare = Time.time + timeToPrepare;
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
        return isFire && shootState.Equals(ShootState.process) && Time.time >= nextPrepare;
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        SaveSystem.TimerSave(jsonObject, "prepare", nextPrepare);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load(jsonObject);
        SaveSystem.TimerLoad(jsonObject, "prepare", ref nextPrepare);      
        return jsonObject;
    }
}
