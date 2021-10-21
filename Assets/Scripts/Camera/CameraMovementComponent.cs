using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementComponent : MonoBehaviour, ISaveableComponent
{
    public float maxWalkSpeed = 10.0f;
    public float speedMultiply = 1.0f;

    [SerializeField]
    protected Vector2 direction = Vector2.down;

    [SerializeField]
    protected float distanceToOffsetPath = 5.0f;
    protected bool hasInput;
    protected bool hasFollow;
    protected Vector3 velocity = Vector3.zero;
    [SerializeField]
    protected float pathArrival—orrection = 0.25f;
    protected bool hasPath, arrivalPath;
    protected int point;
    protected List<Vector3> path = new List<Vector3>();

    public bool HasPath => hasPath;
    public bool ArrivalPath => arrivalPath;
    public Vector2 Direction { get => direction; set => direction = value; }
    public List<Vector3> Path => path;

    public virtual bool Follow(Transform target)
    {
        Vector2 pos1 = target.position;
        Vector2 pos2 = transform.position;
        float distance = (pos1 - pos2).sqrMagnitude;
        Vector2 direction = (pos1 - pos2).normalized;
        return Follow(target, distance, direction);
    }
    public virtual bool Follow(Transform target, float distance, Vector3 direction)
    {
        hasFollow = true;
        Vector2 pos1 = target.position;
        velocity = pos1;
        velocity.z = transform.position.z;
        return distance > pathArrival—orrection * pathArrival—orrection;
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
    public MovePathResult MoveTo(Vector3 point)
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

    public void Teleport(Vector3 point)
    {
        //rigidbody.interpolation = RigidbodyInterpolation2D.None;
        point.z = transform.position.z;
        transform.position = (point);
        //rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
    }
    public void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0)
        {
            velocity = direction * maxWalkSpeed * speedMultiply;
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
            Vector2 pos1 = transform.position;
            Vector2 pos2 = path[point];
            if ((pos1 - pos2).sqrMagnitude <= pathArrival—orrection * pathArrival—orrection)
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
    protected void LateUpdate()
    {
        if (hasFollow)
        {

            Vector3 pos = Vector3.Lerp(transform.position, velocity, maxWalkSpeed * Time.deltaTime);
            pos.z = transform.position.z;
            transform.position = pos;
            hasFollow = false;
        }
        else
        {
            if (hasInput)
            {
                transform.Translate(velocity * Time.deltaTime);
                //rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
                hasInput = false;
            }
            else
            {
                if (hasPath)
                {
                    Vector2 pos1 = transform.position;
                    Vector2 pos2 = path[point];
                    Vector3 direction = (pos2 - pos1).normalized;
                    if (direction.sqrMagnitude > 0)
                        this.direction = direction;
                    velocity = direction * maxWalkSpeed * speedMultiply;
                    transform.Translate(velocity * Time.deltaTime);
                    //rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
                    hasInput = false;
                }
            }
        }

    }
    protected void OnDrawGizmosSelected()
    {
        if (hasPath)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, path[point]);
            Gizmos.color = Color.blue;
            for (int i = 1; i < path.Count; i++)
            {
                Gizmos.DrawLine(path[i - 1], path[i]);
            }
            for (int i = 0; i < path.Count; i++)
            {
                Gizmos.DrawWireSphere(path[i], 0.5f);
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
        jsonObject.Add("hasInput", new JSONBool(hasInput));
        JSONArray velocityJArray = new JSONArray();
        {
            velocityJArray.Add(new JSONNumber(velocity.x));
            velocityJArray.Add(new JSONNumber(velocity.y));
            velocityJArray.Add(new JSONNumber(velocity.z));
        }
        jsonObject.Add("velocity", velocityJArray);
        JSONArray directionJArray = new JSONArray();
        {
            directionJArray.Add(new JSONNumber(direction.x));
            directionJArray.Add(new JSONNumber(direction.y));
        }
        jsonObject.Add("direction", directionJArray);
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
        if (hasPath)
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
        JSONArray velocityJArray = jsonObject["velocity"].AsArray;
        velocity.Set(velocityJArray[0].AsFloat, velocityJArray[1].AsFloat, velocityJArray[2].AsFloat);
        JSONArray directionJArray = jsonObject["direction"].AsArray;
        direction.Set(directionJArray[0].AsFloat, directionJArray[1].AsFloat);
        return jsonObject;
    }
}