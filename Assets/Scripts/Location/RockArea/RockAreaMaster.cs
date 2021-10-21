using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RockAreaMaster : Actor
{
    [SerializeField]
    protected RockAreaObjectComponent rock;
    [SerializeField]
    protected new RockAreaAnimationComponent animation;
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected WaypointHolder waypoints;
    protected int point;
    [SerializeField]
    private float pathArrival—orrection;
    [SerializeField]
    protected bool useInCombat = false;
    [SerializeField]
    protected float damage = 100, power = 100, radiusToDamage = 4.0f;
    protected List<Pawn> targets = new List<Pawn>();
    protected float timeToDamage = 0.05f, nextDamage;


    [ContextMenu("Execute")]
    public void Execute()
    {
        //enabled = true;
        enabled = true;
        rock.OnStart();
        if (useInCombat)
            GameInstance.Instance.GameState.AddInCombat();
    }
    [ContextMenu("Stop")]
    public void Stop()
    {
        enabled = false;
        rock.OnStop();
        if (useInCombat)
            GameInstance.Instance.GameState.RemoveInCombat();
    }
    protected void SetDamage()
    {
        for (int j = 0; j < targets.Count; j++)
        {

            float distance = (rock.Position - targets[j].transform.position).sqrMagnitude;
            if (distance <= radiusToDamage * radiusToDamage)
            {
                Vector2 direction = (targets[j].transform.position - rock.Position).normalized;
                targets[j].Health.Hurt(new DamageStruct(gameObject, Team.world, damage, direction, power), new RaycastHit2D());
            }
        }
    }
    public void Update()
    {
        if (Time.time >= nextDamage)
        {
            SetDamage();
            nextDamage = Time.time + timeToDamage;
        }
        Vector3 p = waypoints.Get(point);
        if ((rock.Position - p).sqrMagnitude <= pathArrival—orrection * pathArrival—orrection)
        {
            point++;
            if (point >= waypoints.Length)
            {
                point = 0;
                Stop();
            }
            else
            {
                Vector3 direction = (p - rock.Position).normalized;
                rock.Position = rock.Position + direction * speed * Time.deltaTime;
            }
        }
        else
        {
            Vector3 direction = (p - rock.Position).normalized;
            rock.Position = rock.Position + direction * speed * Time.deltaTime;
        }
    }
    public void LateUpdate()
    {
        animation.OnLateUpdate();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPerceptionTarget pawn = collision.GetComponent<IPerceptionTarget>();
        if (pawn != null)
        {
            targets.Add(pawn.GetPawn());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        IPerceptionTarget pawn = collision.GetComponent<IPerceptionTarget>();
        if (pawn != null)
        {
            targets.Remove(pawn.GetPawn());
        }
    }
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if(rock != null)
        Gizmos.DrawWireSphere(rock.transform.position, radiusToDamage);
    }
    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        jsonObject.Add("rock", rock.Save(new JSONObject()));
        jsonObject.Add("point", new JSONNumber(point));
        SaveSystem.TimerSave(jsonObject, "damage", nextDamage);
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        rock.Load(jsonObject["rock"].AsObject);
        point = jsonObject["point"].AsInt;
        SaveSystem.TimerLoad(jsonObject, "damage", ref nextDamage);
        return jsonObject;
    }
}