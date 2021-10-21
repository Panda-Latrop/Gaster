using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpikeAreaMaster : Actor
{
    protected bool isPlaying = true;
    [SerializeField]
    protected SpikeAreaGridComponent grid;
    [SerializeField]
    protected new SpikeAreaAnimationComponent animation;
    [SerializeField]
    protected SpikeAreaBehaviourComponent behaviour;
    [SerializeField]
    protected SpikeAreaPatternComponent[] patterns;
    [SerializeField]
    protected int currentBehaviour = 0;
    [SerializeField]
    protected float timeToBehaviour = 2.5f;
    protected float nextBehaviour;
    protected List<Pawn> targets = new List<Pawn>();
    [SerializeField]
    protected float damage = 100.0f, power = 100.0f;
    [SerializeField]
    protected AudioSource source;

    [ContextMenu("Create")]
    public void Create()
    {
        grid.CreateGrid();
        animation.Clear(grid.nodes);
        EditorUtility.SetDirty(this);
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        grid.ClearGrid();
        EditorUtility.SetDirty(this);
    }
    [ContextMenu("Execute")]
    public void Execute()
    {
        enabled = true;
        isPlaying = true;
        behaviour.Pattern = patterns[currentBehaviour];
        nextBehaviour = Time.time + timeToBehaviour;
    }
    [ContextMenu("Stop")]
    public void Stop()
    {
        isPlaying = false;
    }
    protected void SetDamage()
    {
        int count = 0;
        Vector3 audioPos = Vector3.zero;
        for (int i = 0; i < grid.nodes.Length; i++)
        {
            if (grid.nodes[i].state == 1)
            {
                count++;
                audioPos += grid.nodes[i].transform.position;
                for (int j = 0; j < targets.Count; j++)
                {

                    float distance = (grid.nodes[i].transform.position - targets[j].transform.position).sqrMagnitude;
                    if (distance <= 6.5f)
                    {
                        Vector2 direction = (targets[j].transform.position - grid.nodes[i].transform.position).normalized;
                        targets[j].Health.Hurt(new DamageStruct(gameObject, Team.world, damage, direction, power), new RaycastHit2D());
                    }
                }
            }
        }
        audioPos /= count;
        source.transform.position = audioPos;
        source.Stop();
        source.time = 0;
        source.Play();
    }
    public void Update()
    {
        if (isPlaying)
        {
            if (Time.time >= nextBehaviour)
            {
                currentBehaviour++; //Random.Range(0, patterns.Length);
                if (currentBehaviour >= patterns.Length)
                    currentBehaviour = 0;
                behaviour.Pattern = patterns[currentBehaviour];
                nextBehaviour = Time.time + timeToBehaviour;
            }
            else
            {
                if (behaviour.OnUpdate(grid.nodes, grid.gridCount))
                {
                    SetDamage();

                }


            }
        }
    }
    public void LateUpdate()
    {
        if (!animation.OnLateUpdate(grid.nodes) && !isPlaying)
            enabled = false;
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
    /*{
    protected bool isPlaying = true;
    [SerializeField]
    protected SpikeAreaGridComponent grid;
    [SerializeField]
    protected new SpikeAreaAnimationComponent animation;
    [SerializeField]
    protected SpikeAreaBehaviourComponent behaviour;
    [SerializeField]
    protected SpikeAreaPatternComponent[] patterns;
    [SerializeField]
    protected int currentBehaviour = 0;
    [SerializeField]
    protected float timeToBehaviour = 2.5f;
    protected float nextBehaviour;
    protected List<Pawn> targets = new List<Pawn>();*/
    public override JSONObject Save(JSONObject jsonObject)
    {
        base.Save(jsonObject);
        jsonObject.Add("isPlaying", new JSONBool(isPlaying));
        jsonObject.Add("current", new JSONNumber(currentBehaviour));
        SaveSystem.TimerSave(jsonObject, "behaviour", nextBehaviour);
        jsonObject.Add("behaviour", behaviour.Save(new JSONObject()));
        SaveSystem.AudioSourceSave(jsonObject, "main", source);
        return jsonObject;
    }
    public override JSONObject Load(JSONObject jsonObject)
    {
        base.Load(jsonObject);
        isPlaying = jsonObject["isPlaying"].AsBool;
        currentBehaviour = jsonObject["current"].AsInt;
        behaviour.Pattern = patterns[currentBehaviour];
        SaveSystem.TimerLoad(jsonObject, "behaviour",ref nextBehaviour);
        behaviour.Load(jsonObject["behaviour"].AsObject);
        SaveSystem.AudioSourceLoad(jsonObject, "main", source);
        return jsonObject;
    }
}
