using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ShootState
{
    initiated,
    process,
    ended,
    unready,
    none,
}


public abstract class WeaponBase : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected PrefabHolder prefab;
    protected bool isFire;
    [SerializeField]
    protected int slot;
    [SerializeField]
    protected bool twoHanded = true;
    [SerializeField]
    protected bool isAutomatic;
    protected Pawn owner;
    protected bool hasMuzzle;
    [SerializeField]
    protected ParticleSystem muzzleFlash;
    protected bool hasAudio;
    [SerializeField]
    protected AudioSource audioSource;
    [SerializeField]
    protected Transform shootPoint;
    [SerializeField]
    protected float damage = 1.0f;
    public float damageMultiply = 1.0f;
    [SerializeField]
    protected float power = 100.0f, speed = 20.0f;
    [SerializeField]
    protected float fireRate = 1.0f;
    protected float timeToShoot, nextShoot;
    protected ShootState shootState = ShootState.ended;

    public PrefabHolder Prefab => prefab;
    public bool TwoHanded => twoHanded;

    protected virtual void Awake()
    {
        timeToShoot = 1.0f / fireRate;
        hasAudio = audioSource != null;
        hasMuzzle = muzzleFlash != null;
    }
    public int Slot => slot;
    public ShootState ShootState => shootState;
    public void SetOwner(Pawn owner)
    {
        this.owner = owner;
    }
    public virtual void SetFire(bool fire)
    {
        isFire = fire;
    }
    public Transform GetShootPoint()
    {
        return shootPoint;
    }
    protected virtual void PlayAudio()
    {
        if (hasAudio)
        {
            audioSource.time = 0.0f;
            audioSource.Play();
        }          
    }
    protected virtual void PlayMuzzleFlash()
    {
        if (hasMuzzle)
        {
            muzzleFlash.time = 0.0f;
            muzzleFlash.Play(true);
        }
    }
    protected virtual void StopAudio()
    {
        if (hasAudio)
            audioSource.Stop();
    }
    protected virtual void StopMuzzleFlash()
    {
        if (hasMuzzle)
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
    public virtual void SetTarget(Transform target)
    {
        SetTarget(target.transform.position);
        return;
    }
    public virtual void SetTarget(Vector3 target)
    {
        return;
    }
    public abstract ShootState Shoot(Vector3 position, Vector3 direction);
    protected virtual bool CanShoot()
    {
        if (isFire && Time.time >= nextShoot)
        {
            if (!isAutomatic)
                isFire = false;
            nextShoot = Time.time + timeToShoot;
            return true;
        }
        return false;
    }
    protected abstract RaycastHit2D CreateProjectile(Vector3 position, Vector3 direction);
    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("name", new JSONString(gameObject.name));
        jsonObject.Add("prefab", new JSONString(prefab.Path));
        jsonObject.Add("enabled", new JSONBool(enabled));
        jsonObject.Add("active", new JSONBool(gameObject.activeSelf));
        if (hasMuzzle)
            SaveSystem.ParticleSystemSave(jsonObject, "muzzle", muzzleFlash);
        if (hasAudio)
            SaveSystem.AudioSourceSave(jsonObject, "source", audioSource);
        SaveSystem.TimerSave(jsonObject, "next", nextShoot);
        jsonObject.Add("shootState", new JSONNumber((int)shootState));
        jsonObject.Add("damageMultiply", new JSONNumber(damageMultiply));
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        gameObject.name = jsonObject["name"];
        enabled = jsonObject["enabled"].AsBool;
        gameObject.SetActive(jsonObject["active"].AsBool);
        if (hasMuzzle)
            SaveSystem.ParticleSystemLoad(jsonObject, "muzzle", muzzleFlash);
        if (hasAudio)
            SaveSystem.AudioSourceLoad(jsonObject, "source", audioSource);
        SaveSystem.TimerLoad(jsonObject, "next", ref nextShoot);
        shootState = (ShootState)jsonObject["shootState"].AsInt;
        damageMultiply = jsonObject["damageMultiply"].AsFloat;
        return jsonObject;
    }
}
