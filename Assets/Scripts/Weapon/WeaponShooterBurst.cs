using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShooterBurst : WeaponShooter
{
    protected bool inBrust;
    [SerializeField]
    protected int brustShootCount = 3;
    protected int currentBrustCount;
    [SerializeField]
    protected float timeToShootBrust;
    protected float nextBrust;

    protected void OnDisable()
    {
        inBrust = false;
        currentBrustCount = 0;
    }
    public override ShootState Shoot(Vector3 position, Vector3 direction)
    {
        if (CanShoot())
        {
            enabled = true;
            CreateProjectile(position, direction);
            IncreaseSpread();
            PlayMuzzleFlash();
            PlayAudio();
            currentBrustCount++;
            nextBrust = Time.time + timeToShootBrust;
            inBrust = true;
            shootState = ShootState.initiated;
            return shootState;
        }
        shootState = ShootState.unready;
        return shootState;
    }
    protected override void Update()
    {
        if(!inBrust)
        base.Update();
        else
        {
            if(Time.time >= nextBrust)
            {            
                CreateProjectile(shootPoint.transform.position, shootPoint.transform.right);
                IncreaseSpread();
                PlayMuzzleFlash();
                PlayAudio();
                currentBrustCount++;                
                nextBrust = Time.time + timeToShootBrust;
                shootState = ShootState.process;
                if(currentBrustCount >= brustShootCount)
                {
                    inBrust = false;
                    currentBrustCount = 0;
                    shootState = ShootState.ended;
                }
            }
            
        }
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("inBrust", new JSONBool(inBrust));
        jsonObject.Add("brustCount", new JSONNumber(currentBrustCount));
        SaveSystem.TimerSave(jsonObject, "brust",nextBrust);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load(jsonObject);
        inBrust = jsonObject["inBrust"].AsBool;
        currentBrustCount = jsonObject["brustCount"].AsInt;
        SaveSystem.TimerLoad(jsonObject, "brust",ref nextBrust);
        return jsonObject;
    }





}
