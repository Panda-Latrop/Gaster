using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaMaster : Actor
{
    protected Action OnEnd;

    [SerializeField]
    protected List<ArenaSpawnBase> spawns = new List<ArenaSpawnBase>();
    [SerializeField]
    protected List<string> wavePatterns = new List<string>();
    protected int currentWave = 0;
    protected List<Pawn> pawns = new List<Pawn>();
    [SerializeField]
    protected float timeToWave = 1.0f;
    protected float nextWave;
    [SerializeField]
    protected List<ArenaObstacle> obstacles = new List<ArenaObstacle>();

    protected int WavePatternIndex(int i) => wavePatterns[currentWave][i] - 48;

    [ContextMenu("Start Arena")]
    public void Execute()
    {
        enabled = true;
        nextWave = Time.time + timeToWave;
        currentWave = 0;      
        for (int i = 0; i < wavePatterns[currentWave].Length; i++)
        {
            spawns[WavePatternIndex(i)].OnStart();
        }
        GameInstance.Instance.GameState.AddInCombat();
        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].Show();
        }
    }
    protected void Finish()
    {
        enabled = false;
        CallOnEnd();
        GameInstance.Instance.GameState.RemoveInCombat();
        for (int i = 0; i < obstacles.Count; i++)
        {
            obstacles[i].Hide();
        }
        Debug.Log("Arena End");
    }

    protected void Update()
    {
        if (Time.time >= nextWave)
        {
            for (int i = 0; i < wavePatterns[currentWave].Length; i++)
            {
                ArenaSpawnBase asb = spawns[WavePatternIndex(i)];
                if (asb.IsFinished)
                    continue;
                else
                {
                    asb.OnSpawn(ref pawns);
                }
            }
            if (WaveIsEnd())
            {
                if (!Next())
                {
                    Finish();
                }
            }
            for (int i = pawns.Count - 1; i >= 0; i--)
            {
                if (!pawns[i].Health.IsAlive || !pawns[i].Health.IsMorale)
                {
                    pawns.RemoveAt(i);
                }
            }
        }
    }
    public bool Next()
    {
        currentWave++;
        nextWave = Time.time + timeToWave;
        if (currentWave < wavePatterns.Count)
        {
            for (int i = 0; i < wavePatterns[currentWave].Length; i++)
                spawns[WavePatternIndex(i)].OnStart();
            return true;
        }
        return false; ;
    }
    protected bool WaveIsEnd()
    {
        bool finished = false;
        for (int i = 0; i < wavePatterns[currentWave].Length; i++)
        {
            ArenaSpawnBase asb = spawns[WavePatternIndex(i)];
            if (!(finished = asb.IsFinished || (!asb.IsFinished && asb.InfinitySpawn)))
                break;

        }
        return finished && pawns.Count <= 0;
    }
    protected void OnDestroy()
    {
        OnEnd = null;
    }
    public void CallOnEnd()
    {
        OnEnd?.Invoke();
    }
    public void BindOnEnd(Action action)
    {
        OnEnd += action;
    }
    public void UnbindOnEnd(Action action)
    {
        OnEnd -= action;
    }
    public void ClearOnEnd()
    {
        OnEnd = null;
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("currentWave", new JSONNumber(currentWave));
        JSONArray spawnsJArray = new JSONArray();
        for (int i = 0; i < spawns.Count; i++)
            spawnsJArray.Add(spawns[i].Save(new JSONObject()));
        jsonObject.Add("spawns", spawnsJArray);
        JSONArray pawnsJArray = new JSONArray();
        for (int i = 0; i < pawns.Count; i++)
        {
            if (pawns[i].Health.IsAlive && pawns[i].Health.IsMorale)
            {
                JSONObject pawnJObject = new JSONObject();
                SaveSystem.ComponentReferenceSave(pawnJObject, "pawn", pawns[i]);
                pawnsJArray.Add(pawnJObject);
            }
        }
        jsonObject.Add("pawns", pawnsJArray);
        JSONArray obstaclesJArray = new JSONArray();
        for (int i = 0; i < obstacles.Count; i++)
            obstaclesJArray.Add(obstacles[i].Save(new JSONObject()));
        jsonObject.Add("obstacles", obstaclesJArray);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        currentWave = jsonObject["currentWave"].AsInt;
        JSONArray spawnsJArray = jsonObject["spawns"].AsArray;
        for (int i = 0; i < spawnsJArray.Count; i++)
            spawns[i].Load(spawnsJArray[i].AsObject);
        JSONArray pawnsJArray = jsonObject["pawns"].AsArray;
        for (int i = 0; i < pawnsJArray.Count; i++)
        {
            JSONObject pawnJObject = pawnsJArray[i].AsObject;
            Pawn p = default;
            if (SaveSystem.ComponentReferenceLoad(pawnJObject, "pawn",ref p))
                pawns.Add(p);
        }
        JSONArray obstaclesJArray = jsonObject["obstacles"].AsArray;
        for (int i = 0; i < obstaclesJArray.Count; i++)
            obstacles[i].Load(obstaclesJArray[i].AsObject);
        return jsonObject;
    }
}
