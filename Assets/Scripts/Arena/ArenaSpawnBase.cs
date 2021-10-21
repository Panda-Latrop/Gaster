using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArenaSpawnBase : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected int maxSpawn;
    [SerializeField]
    protected List<Vector3> spawnPoints = new List<Vector3>();
    [SerializeField]
    protected float spawnRadius = 2.0f;
    [SerializeField]
    protected bool useSequenceSpawn;
    [SerializeField]
    protected float timeToSpawn = 0.5f;
    [SerializeField]
    protected bool infinitySpawn;
    [SerializeField]
    protected bool useSpawnEffect;
    [SerializeField]
    protected DynamicExecutor spawnEffect;
    protected float nextSpawn;
    protected int spawned, spawnPointShift;
    protected bool isFinished;
    public bool IsFinished => isFinished;
    public bool InfinitySpawn => infinitySpawn;
    public virtual void OnStart()
    {
        spawnPointShift = Random.Range(0, spawnPoints.Count);
        isFinished = false;
        spawned = 0;
    }
    public abstract void OnSpawn(ref List<Pawn> pawns);   
    public abstract void OnEnd();
    protected abstract Pawn Spawn();
    protected virtual bool CanSpawn()
    {
        if (Time.time >= nextSpawn)
        {
            nextSpawn = Time.time + timeToSpawn;
            return true;
        }
        return false;
    }
    protected virtual Vector3 RandomPoint()
    {
        Vector3 point = spawnPoints[spawnPointShift] + (Vector3)Random.insideUnitCircle*spawnRadius;
        spawnPointShift++;
        if (spawnPointShift >= spawnPoints.Count)
            spawnPointShift = 0;
        return point;
    }
    protected virtual bool SpawnLimit()
    {
        return spawned >= maxSpawn;
    }
    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("spawned", new JSONNumber(spawned));
        jsonObject.Add("spawnPointShift", new JSONNumber(spawnPointShift));
        jsonObject.Add("isFinished", new JSONBool(isFinished));
        SaveSystem.TimerSave(jsonObject, "spawn", nextSpawn);
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        spawned = jsonObject["spawned"].AsInt;
        spawnPointShift = jsonObject["spawnPointShift"].AsInt;
        isFinished = jsonObject["isFinished"].AsBool;
        SaveSystem.TimerLoad(jsonObject, "spawn",ref nextSpawn);
        return jsonObject;
    }
}