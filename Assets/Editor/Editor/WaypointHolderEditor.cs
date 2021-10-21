using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaypointHolder))]
public class WaypointHolderEditor : Editor
{

    WaypointHolder holder;
    float size = 0.25f;

    protected void OnEnable()
    {
        holder = target as WaypointHolder;
    }

    protected virtual void OnSceneGUI()
    {
        Debug.Log("Call");
        if (holder.Length > 0)
        {
            Vector3 snap = Vector3.one * 0.5f;

            for (int i = 0; i < holder.Length; i++)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition = Handles.FreeMoveHandle(holder.Get(i), Quaternion.identity, size, snap, Handles.RectangleHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(holder, "Change Waypoint");
                    holder.Set(i, newTargetPosition);
                }
            }
        }
    }
}