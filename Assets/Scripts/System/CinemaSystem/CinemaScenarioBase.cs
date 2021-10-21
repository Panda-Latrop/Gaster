using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaScenarioBase : MonoBehaviour
{
    protected Action OnEnd;

    [SerializeField]
    protected bool overrideInput = true;
    public float duration;
    protected float time;
    protected bool isPlaying;

    [SerializeField]
    protected List<CinemaBaseComponent> nodes = new List<CinemaBaseComponent>();
    protected int currentNode;
    protected CinemaBaseComponent node => nodes[currentNode];
    public List<CinemaBaseComponent> Nodes => nodes;

    public bool OverrideInput => overrideInput;

/*#if UNITY_EDITOR
    [ContextMenu("Auto Set")]
    protected void AutoSet()
    {
        nodes = new List<CinemaBaseComponent>(gameObject.GetComponents<CinemaBaseComponent>());
        nodes.Sort();
        Debug.Log((nodes[nodes.Count - 1].start + 0.1f));
        duration = nodes[nodes.Count - 1].start + 0.1f;
        
    }

#endif*/
    [ContextMenu("Play")]
    public void Play()
    {
        time = 0;
        currentNode = 0;
        enabled = isPlaying = true;
    }
    public void Skip()
    {
        for (int i = 0; i < nodes.Count; i++)
            nodes[i].OnSkip();
        Stop();
    }
    public void Stop()
    {
        time = 0;
        enabled = isPlaying = false;
        CallOnEnd();
    }



    protected void Check()
    {
        if (currentNode < nodes.Count && time >= node.start)
        {
            node.OnPlay();
            if ((isPlaying = !node.waitForEnd) && Change())
            {
                Check();
            }
        }
    }

    protected bool Change()
    {
        return (++currentNode < nodes.Count);   
    }

    public void OnUpdate()
    {
        for (int i = 0; i < currentNode; i++)
        {
            if (!nodes[i].Finished)
            {
                //Debug.Log(i);
                if (nodes[i].OnUpdate())
                {
                    nodes[i].OnEnd();
                }
            }
        }
        if (isPlaying)
        {
            Check();
        }
        if (!isPlaying)
        {
            if (node.waitForEnd && node.OnUpdate())
            {
                node.OnEnd();
                isPlaying = true;
                Change();
                Check();
            }
        }

    }

    public bool OnLateUpdate()
    {

        if (isPlaying)
        {
            time += Time.deltaTime;
            if (time >= duration)
            {
                time = duration;
                Stop();
                return false;
            }
        }
        return true;
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
}
