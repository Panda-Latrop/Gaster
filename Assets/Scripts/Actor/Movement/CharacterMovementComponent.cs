using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovePathResult
{
    process,
    done,
    fail
}

public class CharacterMovementComponent : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected new Rigidbody2D rigidbody;

    public float maxWalkSpeed = 10.0f;
    public float speedMultiply = 1.0f;

    [SerializeField]
    protected Vector2 direction = Vector2.down;

    [SerializeField]
    protected float distanceToFocus = 5.0f;
    [SerializeField]
    protected float distanceToOffsetPath = 5.0f;
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    protected float pushResistance = 0;
    protected bool hasInput, hasPush;
    protected Vector2 velocity = Vector2.zero;
    [SerializeField]
    protected float pathArrival—orrection = 0.25f;
    protected bool hasPath, arrivalPath;
    protected int point;
    protected List<Vector3> path = new List<Vector3>();
    [SerializeField]
    protected float timeToRebuildPath = 3.0f;
    protected float nextRebuild;




    public bool HasPath => hasPath;
    public bool ArrivalPath => arrivalPath;
    public Rigidbody2D Rigidbody => rigidbody;
    public Vector2 Direction { get => direction; set => direction = value; }
    public List<Vector3> Path => path;

    public virtual bool FollowTarget(Pawn target)
    {
        float distance = (target.transform.position - transform.position).sqrMagnitude;
        Vector3 direction = (target.transform.position - transform.position).normalized;
        return FollowTarget(target, distance, direction);
    }
    public virtual bool FollowTarget(Pawn target, float distance, Vector3 direction)
    {
        if (distance > distanceToFocus * distanceToFocus)
        {
           // Debug.Log(gameObject.name + "1");
            float offsetDistance = 0.0f;
            if (hasPath)
                offsetDistance = (target.transform.position - path[0]).sqrMagnitude;
            if (Time.time >= nextRebuild ||
                arrivalPath ||
                offsetDistance >= distanceToOffsetPath * distanceToOffsetPath)
            {
                //Debug.Log(gameObject.name + "2");
                MoveTo(target.transform.position, true);
                nextRebuild = Time.time + timeToRebuildPath;
            }
            return false;
        }
        else
        {
            if (distance > 1.5f)
            {
                Move(direction);
            }
            return true;
        }
    }

   /* public void MoveTo(Vector3 point, bool forceChangePath = false)
    {

    }*/

        public MovePathResult MoveTo(Vector3 point,bool forceChangePath = false)
    {
        if (!hasPath || forceChangePath)
        {
            if (GameInstance.Instance.Navigation.FindPath(rigidbody.position, point, ref path))
            {
                hasPath = true;
                hasInput = false;
                this.point = path.Count-1;
                arrivalPath = false;
                return MovePathResult.process;
            }
            else
            {             
                this.point = -1;
                hasPath = false;
                arrivalPath = true;
                return MovePathResult.fail;
            }
        }
        else
        {
            if (this.point < 0)
            {
                this.point = -1;
                hasPath = false;
                arrivalPath = true;
                return MovePathResult.done;
            }
            else
            {
                return MovePathResult.process;
            }
        }

    }
    public MovePathResult MoveTo(List<Vector3> path)
    {
        if (!hasPath)
        {
            hasPath = true;
            hasInput = false;
            this.path = path;
            point = this.path.Count - 1;
            arrivalPath = false;
            return MovePathResult.process;
        }
        else
        {
            if (point < 0)
            {
                point = -1;
                hasPath = false;
                arrivalPath = true;
                return MovePathResult.done;
            }
            else
            {
                return MovePathResult.process;
            }
        }

    }
    public MovePathResult MoveToSingle(Vector3 point)
    {
        if (!hasPath)
        {
            path.Clear();
            hasPath = true;
            hasInput = false;
            path.Add(point);
            this.point = path.Count - 1;
            arrivalPath = false;
            return MovePathResult.process;
        }
        else
        {
            if (this.point < 0)
            {
                this.point = -1;
                hasPath = false;
                arrivalPath = true;
                return MovePathResult.done;
            }
            else
            {
                return MovePathResult.process;
            }
        }
    }

    public void Teleport(Vector2 point)
    {
        rigidbody.position = (point);
    }
    public void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0 && !hasPush)
        {
            velocity = direction * maxWalkSpeed* speedMultiply;
            this.direction = direction;
            hasInput = true;        
        }
        else
        {
            hasInput = false;
        }
       
        hasPath = false;
        arrivalPath = true;
    }
    public void Push(Vector3 direction,float power)
    {
        power *= (1 - pushResistance);
        if (direction.sqrMagnitude > 0)
        {            
            velocity = direction * power;
            this.direction = direction;
            hasPush = true;
        }
        else
        {
            hasPush = false;
        }
    }
    public void Stop()
    {
        hasInput = false;
        hasPath = false;
        arrivalPath = true;
    }
    protected void Update()
    {
        if (hasPath)
        {
            if (((Vector3)rigidbody.position - path[point]).sqrMagnitude <= pathArrival—orrection * pathArrival—orrection)
            {
                point--;
                if (point < 0)
                {
                    this.point = -1;
                    hasPath = false;
                    arrivalPath = true;
                }
            }
        }
    }
    protected void FixedUpdate()
    {
        if (hasPush)
        {
            rigidbody.velocity = velocity;
            hasPush = false;
        }
        else
        {
            if (hasInput)
            {
                rigidbody.velocity = velocity;
                hasInput = false;
            }
            else
            {
                if (hasPath)
                {
                    Vector3 direction = (path[point] - (Vector3)rigidbody.position).normalized;
                    if (direction.sqrMagnitude > 0)
                        this.direction = direction;
                    velocity = direction * maxWalkSpeed* speedMultiply;
                    rigidbody.velocity = velocity;
                    hasInput = false;
                }
                else
                {
                    rigidbody.velocity = Vector2.zero;
                }
            }
        }
        
    }
    protected void OnDrawGizmosSelected()
    {
        if (hasPath)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rigidbody.position, path[point]);
            Gizmos.color = Color.blue;
            for (int i = 1; i < path.Count; i++)
            {
                Gizmos.DrawLine(path[i - 1], path[i]);
            }
            for (int i = 0; i < path.Count; i++)
            {
                Gizmos.DrawWireSphere(path[i],0.5f);
            }
        }
    }
    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("enabled", enabled);
        jsonObject.Add("maxWalkSpeed", new JSONNumber(maxWalkSpeed));
        jsonObject.Add("speedMultiply", new JSONNumber(speedMultiply));
        jsonObject.Add("pathArrival—orrection", new JSONNumber(pathArrival—orrection));
        jsonObject.Add("hasPath", new JSONBool(hasPath));
        jsonObject.Add("arrivalPath", new JSONBool(arrivalPath));
        jsonObject.Add("point", new JSONNumber(point));
        if (hasPath)
        {
            JSONArray pathJArray = new JSONArray();
            {
                for (int i = 0; i < path.Count; i++)
                {
                    JSONArray pointJArray = new JSONArray();
                    pointJArray.Add(new JSONNumber(path[i].x));
                    pointJArray.Add(new JSONNumber(path[i].y));
                    pointJArray.Add(new JSONNumber(path[i].z));
                    pathJArray.Add(pointJArray);
                }
            }
            jsonObject.Add("path", pathJArray);
        }
        jsonObject.Add("hasInput", new JSONBool(false));
        jsonObject.Add("hasPush", new JSONBool(hasPush));       
        JSONArray velocityJArray = new JSONArray();
        {
            velocityJArray.Add(new JSONNumber(velocity.x));
            velocityJArray.Add(new JSONNumber(velocity.y));            
        }
        jsonObject.Add("velocity", velocityJArray);
        JSONArray directionJArray = new JSONArray();
         {
            directionJArray.Add(new JSONNumber(direction.x));
            directionJArray.Add(new JSONNumber(direction.y));
         }
        jsonObject.Add("direction", directionJArray);
        SaveSystem.TimerSave(jsonObject, "rebuild", nextRebuild);
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        enabled = jsonObject["enabled"].AsBool;
        maxWalkSpeed = jsonObject["maxWalkSpeed"].AsFloat;
        speedMultiply = jsonObject["speedMultiply"].AsFloat;       
        pathArrival—orrection = jsonObject["pathArrival—orrection"].AsFloat;
        hasPath = jsonObject["hasPath"].AsBool;
        arrivalPath = jsonObject["arrivalPath"].AsBool;
        point = jsonObject["point"].AsInt;
        if(hasPath)
        {
            path.Clear();
            JSONArray pathJArray = jsonObject["path"].AsArray;
            for (int i = 0; i < pathJArray.Count; i++)
            {              
                JSONArray pointJArray = pathJArray[i].AsArray;
                path.Add(new Vector3(pointJArray[0].AsFloat, pointJArray[1].AsFloat, pointJArray[2].AsFloat));
            }
        }
        hasInput = jsonObject["hasInput"].AsBool;
        hasPush = jsonObject["hasPush"].AsBool;
        JSONArray velocityJArray = jsonObject["velocity"].AsArray;
        velocity.Set(velocityJArray[0].AsFloat, velocityJArray[1].AsFloat);
        JSONArray directionJArray = jsonObject["direction"].AsArray;
        direction.Set(directionJArray[0].AsFloat, directionJArray[1].AsFloat);
        SaveSystem.TimerLoad(jsonObject, "rebuild",ref nextRebuild);
        return jsonObject;
    }

}