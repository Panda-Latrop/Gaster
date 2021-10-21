using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicLineRenderer : DynamicExecutor, IPoolDynamicLineRenderer
{
    [SerializeField]
    protected LineRenderer lineRenderer;
    protected List<GameObject> points = new List<GameObject>();
    [SerializeField]
    protected float renderTime = 1.0f;
    protected float nextRender;
    // public LineRenderer LineRenderer => lineRenderer;

    public void SetPoint(int index, GameObject point)
    {
       if(index < points.Count)
        {
            points[index] = point;
        }
        else
        {
            points.Insert(index, point);
        }
    }
    protected override void LateUpdate()
    {
        if (lineRenderer.enabled && Time.time >= nextRender)
        {
            lineRenderer.enabled = false;
        }
        base.LateUpdate();
        for (int i = 0; i < points.Count && i < lineRenderer.positionCount; i++)
        {
            if(!points[i].activeSelf)
            {
                Push();
                return;
            }
            lineRenderer.SetPosition(i, points[i].transform.position);
        }

    }
    public override void OnPop()
    {
        base.OnPop();
        lineRenderer.enabled = true;
        nextRender = Time.time + renderTime;
    }
    public override void OnPush()
    {
        base.OnPush();
        points.Clear();
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        SaveSystem.TimerSave(jsonObject, "render", nextRender);
        SaveSystem.LineRendererSave(jsonObject, "main",lineRenderer);
        JSONArray pointsJArray = new JSONArray();
        {
            for (int i = 0; i < points.Count; i++)
            {
                JSONObject pointJObject = new JSONObject();
                SaveSystem.GameObjectReferenceSave(pointJObject, "point", points[i]);
                pointsJArray.Add(pointJObject);
            }
        }
        jsonObject.Add("points", pointsJArray);
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        SaveSystem.TimerLoad(jsonObject, "render", ref nextRender);
        SaveSystem.LineRendererLoad(jsonObject, "main", lineRenderer);
        JSONArray pointsJArray = jsonObject["points"].AsArray;
        {
            for (int i = 0; i < pointsJArray.Count; i++)
            {
                JSONObject pointJObject = pointsJArray[i].AsObject;
                GameObject go = default;
                if(SaveSystem.GameObjectReferenceLoad(pointJObject, "point",ref go))
                {
                    points.Add(go);
                }
            }
        }
        return jsonObject;
    }
}
