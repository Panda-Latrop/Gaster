using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HurtResult
{
    miss,
    friend,
    enemy,
    kill
}
public enum Team
{
    world,
    team0,
    team1,
    team2,
}
public interface IHealth
{

    HurtResult Hurt(DamageStruct ds, RaycastHit2D raycastHit);
    void Heal(float health);
    HurtResult DownMorale(DamageStruct ds, RaycastHit2D raycastHit);
    void UpMorale(float morale);
    Team Team { get; set; }
}
[System.Serializable]
public struct DamageStruct
{
    public GameObject causer;
    public Team team;
    public float damage;
    public Vector3 direction;
    public float power;
    public DamageStruct(GameObject causer, Team team, float damage, Vector3 direction, float power)
    {
        this.causer = causer;
        this.team = team;
        this.damage = damage;
        this.direction = direction;
        this.power = power;
    }
}
public delegate void DamageDelegate(DamageStruct ds, RaycastHit2D raycastHit);
public class HealthComponent : MonoBehaviour , IHealth, ISaveableComponent
{
    [SerializeField]
    protected bool isAlive = true;
    [SerializeField]
    protected bool isImmortal;
    [SerializeField]
    protected float maxHealth = 100;
    protected float currentHealth = 0;
    [SerializeField]
    protected bool isMorale = true;
    [SerializeField]
    protected bool isAmoral;
    [SerializeField]
    protected float maxMorale = 100;
    protected float currentMorale = 0;
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    protected float fullResistance = 0.0f;
    protected float currentResistance = -2.0f;
    [SerializeField]
    protected Team team;
    protected Team currentTeam;

    protected DamageDelegate OnHurt;
    protected DamageDelegate OnDeath;
    protected DamageDelegate OnMorale;
    protected System.Action OnResurrect;

    public bool IsAlive { get => isAlive; }
    public bool IsImmortal { get => isImmortal; }
    public float Health { get => currentHealth; set {currentHealth = value;if (currentHealth > maxHealth) currentHealth = maxHealth;} }
    public float MaxHealth => maxHealth;
    public bool IsMorale { get => isMorale; }
    public bool IsAmoral { get => isAmoral; }
    public float Morale { get => currentMorale; set { currentMorale = value; if (currentMorale > maxMorale) currentMorale = maxMorale; } }
    public float MaxMorale => maxMorale;
    public float FullResistance { get => currentResistance; set { currentResistance = Mathf.Clamp(value ,- 1.0f,1.0f); } }
    public Team Team { get => currentTeam;  set => currentTeam = value; }
    public void Start()
    {
        if (currentHealth <= 0)
            currentHealth = maxHealth;
        if (currentMorale <= 0)
            currentMorale = maxMorale;
        if (currentTeam.Equals(Team.world))
            currentTeam = team;
        if (currentResistance == -2)
            currentResistance = fullResistance;
    }
    public HurtResult Hurt(DamageStruct ds, RaycastHit2D raycastHit)
    {
        if (isAlive)
        {
            if (ds.team != currentTeam || (ds.team == Team.world && currentTeam == Team.world))
            {
                if (!isImmortal)
                {
                    currentHealth = currentHealth - ds.damage * (1 - fullResistance);
                    if (currentHealth <= 0)
                    {
                        isAlive = false;
                        CallOnDeath(ds, raycastHit);
                        return HurtResult.kill;
                    }
                }
                CallOnHurt(ds, raycastHit);
                return HurtResult.enemy;
            }
            else
            {
                return HurtResult.friend;
            }
        }
        return HurtResult.miss;
    }
    public void Heal(float health)
    {
        if (isAlive)
        {
            currentHealth += health;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }
    }
    public HurtResult DownMorale(DamageStruct ds, RaycastHit2D raycastHit)
    {
        if (isMorale)
        {
            if (ds.team != team || (ds.team == Team.world && team == Team.world))
            {
                if (!isAmoral)
                {
                    currentMorale -= ds.damage;
                    if (currentMorale <= 0)
                    {
                        isMorale = false;
                        CallOnMorale(ds, raycastHit);
                        return HurtResult.kill;
                    }                  
                }
                //CallOnMorale(ds, raycastHit);
                return HurtResult.enemy;
            }
            else
            {
                return HurtResult.friend;
            }
        }
        return HurtResult.miss;

    }
    public void UpMorale(float morale)
    {
        if (isMorale)
        {
            currentMorale += morale;
            if (currentMorale > maxMorale)
                currentMorale = maxMorale;
        }
    }
    [ContextMenu("Kill")]
    public HurtResult Kill()
    {
        if (isAlive)
        {
            isAlive = false;
            currentHealth = 0;
            currentMorale = 0;
            CallOnDeath(new DamageStruct(gameObject, Team.world, maxHealth, Vector3.zero, 0), new RaycastHit2D());
            return HurtResult.kill;
        }
        return HurtResult.miss;
    }
    public void Resurrect()
    {
        if (!isAlive)
        {
            isAlive = true;
            isMorale = true;
            currentHealth = maxHealth;
            currentMorale = maxMorale;
            currentResistance = fullResistance;
            currentTeam = team;
            CallOnResurrect();
        }
    }

    protected void OnDestroy()
    {
        OnHurt = null;
        OnDeath = null;
        OnMorale = null;
        OnResurrect = null;
    }
    #region Delegate
    public void CallOnHurt(DamageStruct ds, RaycastHit2D raycastHit)
    {
        OnHurt?.Invoke(ds, raycastHit);
    }
    public void BindOnHurt(DamageDelegate action)
    {
        OnHurt += action;
    }
    public void UnbindOnHurt(DamageDelegate action)
    {
        OnHurt -= action;
    }
    public void ClearOnHurt()
    {
        OnHurt = null;
    }

    public void CallOnDeath(DamageStruct ds, RaycastHit2D raycastHit)
    {
        OnDeath?.Invoke(ds, raycastHit);
    }
    public void BindOnDeath(DamageDelegate action)
    {
        OnDeath += action;
    }
    public void UnbindOnDeath(DamageDelegate action)
    {
        OnDeath -= action;
    }
    public void ClearOnDeath()
    {
        OnDeath = null;
    }
    public void CallOnMorale(DamageStruct ds, RaycastHit2D raycastHit)
    {
        OnMorale?.Invoke(ds, raycastHit);
    }
    public void BindOnMorale(DamageDelegate action)
    {
        OnMorale += action;
    }
    public void UnbindOnMorale(DamageDelegate action)
    {
        OnMorale -= action;
    }
    public void ClearOnMorale()
    {
        OnMorale = null;
    }
    public void CallOnResurrect()
    {
        OnResurrect?.Invoke();
    }
    public void BindOnResurrect(System.Action action)
    {
        OnResurrect += action;
    }
    public void UnbindOnResurrect(System.Action action)
    {
        OnResurrect -= action;
    }
    public void ClearOnResurrect()
    {
        OnResurrect = null;
    }
    #endregion
    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("isAlive", new JSONBool(isAlive));
        jsonObject.Add("currentHealth", new JSONNumber(currentHealth));
        jsonObject.Add("isMorale", new JSONBool(isMorale));
        jsonObject.Add("currentMorale", new JSONNumber(currentMorale));
        jsonObject.Add("currentResistance", new JSONNumber(currentResistance));
        jsonObject.Add("team", new JSONNumber((int)currentTeam));
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        isAlive = jsonObject["isAlive"].AsBool;
        currentHealth = jsonObject["currentHealth"].AsFloat;
        isMorale = jsonObject["isMorale"].AsBool;
        currentMorale = jsonObject["currentMorale"].AsFloat;
        currentResistance = jsonObject["currentResistance"].AsFloat;
        currentTeam = (Team)jsonObject["team"].AsInt;
        return jsonObject;
    }


}
