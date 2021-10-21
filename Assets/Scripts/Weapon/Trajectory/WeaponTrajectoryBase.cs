using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponTrajectoryBase : WeaponBase
{
    public enum StageState
    {
        stage0,
        stage1,
        stage2,
        stage3
    }

    [SerializeField]
    protected ProjectileBase projectile;
    [SerializeField]
    protected int maxProjectileCount = 10;
    protected List<ProjectileBase> projectiles = new List<ProjectileBase>();
    protected List<bool> ignore = new List<bool>();
    protected StageState stage;

    [SerializeField]
    protected WeaponTrajectoryBaseComponent trajectoryComponent;
    [SerializeField]
    protected bool lookAlongTrajectory = false;
    [SerializeField]
    protected float orientationShift = 0;
    protected Vector3 last;
    [SerializeField]
    protected bool pushOnRelease = false;
    [SerializeField]
    protected bool useRandomShift;
    [SerializeField]
    protected bool animateScale = false;
    [SerializeField]
    protected float minScale = 0.0f, maxScale = 1.0f,speedScale = 1.0f;

    public void AddProjectile(ProjectileBase projectile)
    {
        projectiles.Add(projectile);
        ignore.Add(false);
    }

    protected virtual void OnProjectileCountChange()
    {
        trajectoryComponent.SetCount(projectiles.Count);
    }
    protected virtual void FollowTrajectory()
    {
        for (int i = projectiles.Count-1; i>= 0; i--)
        {
            if (!ignore[i])
            {
                if (!projectiles[i].enabled)
                {
                    ignore[i] = true;
                    continue;
                }
                else
                {
                    
                    Vector3 traj = trajectoryComponent.Evaluate(i);
                    float angle = 0;
                    if (lookAlongTrajectory)
                    {
                        Vector2 n = traj- transform.position;
                       // n.Normalize();
                        angle = Mathf.Atan2(n.y, n.x)*Mathf.Rad2Deg;
                    }
                        
                    angle += orientationShift;
                    projectiles[i].Rigidbody.MoveRotation(angle);
                    projectiles[i].Rigidbody.MovePosition(Vector2.Lerp(projectiles[i].transform.position, traj, 0.45f));
                    //last = traj;
                }
            }
        }      
    }
    protected bool IsAllIgnore()
    {
        for (int i = 0; i < ignore.Count; i++)
        {
            if (!ignore[i])
                return false;
        }
        return true;
    }
    protected virtual bool ReleaseProjectile(Vector3 direction)
    {
        int last = projectiles.Count - 1;
        while (projectiles.Count > 0 && ignore[last])
        {
            projectiles.RemoveAt(last);
            last--;
        }
        if (projectiles.Count > 0)
        {
            if (pushOnRelease)
                projectiles[last].OnPush();
            else
                projectiles[last].SetDirection(direction, 0);
            projectiles.RemoveAt(last);
            return projectiles.Count > 0;
        }
        return false;
    }
    protected virtual void ReleaseAllProjectiles()
    {
        for (int i = 0; i < projectiles.Count; i++)
        {
            if (!ignore[i])
            {
                if (pushOnRelease)
                {
                    projectiles[i].OnPush();
                }
                   
                else
                {
                    Vector2 direction = (projectiles[i].transform.position - transform.position).normalized;
                    projectiles[i].SetDirection(direction, 0);
                }
            }
        }
        projectiles.Clear();
        ignore.Clear();
    }

    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("projectileCount", new JSONNumber(projectiles.Count));
        JSONArray projectilesJArray = new JSONArray();
        for (int i = 0; i < projectiles.Count; i++)
        {
            JSONObject projectileJObject = new JSONObject();
            projectileJObject.Add("ignore",new JSONBool(ignore[i] || !projectiles[i].enabled));
            if (!ignore[i] && projectiles[i].enabled)
            {
                SaveSystem.ComponentReferenceSave(projectileJObject, "projectile", projectiles[i]);              
            }
            projectilesJArray.Add(projectileJObject);
        }
        jsonObject.Add("projectiles", projectilesJArray);
        jsonObject.Add("stage", new JSONNumber((int)stage));
        jsonObject.Add("trajectory", trajectoryComponent.Save(new JSONObject()));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load(jsonObject);
        int projectileCount = jsonObject["projectileCount"].AsInt;
        JSONArray projectilesJArray = jsonObject["projectiles"].AsArray;
        for (int i = 0; i < projectileCount; i++)
        {
            JSONObject projectileJObject = projectilesJArray[i].AsObject;
            ignore.Add(projectileJObject["ignore"].AsBool);
            if (!ignore[i])
            {
                ProjectileBase p = default;
                if(!(ignore[i] = !SaveSystem.ComponentReferenceLoad(projectileJObject, "projectile",ref p)))
                projectiles.Add(p);
            }
            else
            {
                projectiles.Add(null);
            }
        }
        stage = (StageState)jsonObject["stage"].AsInt;
        trajectoryComponent.Load(jsonObject["trajectory"].AsObject);
        return jsonObject;
    }
}