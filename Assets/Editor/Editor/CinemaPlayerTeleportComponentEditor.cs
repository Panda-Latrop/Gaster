using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CinemaPlayerTeleportComponent))]
public class CinemaPlayerTeleportComponentEditor : Editor
{
    protected bool toggle;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var spStart = serializedObject.FindProperty("start");
        var spWFE = serializedObject.FindProperty("waitForEnd");
        var spPoint = serializedObject.FindProperty("point");

        StringBuilder data = new StringBuilder("Data: PLAYER");
        data.Append(", ").Append("To: ").Append(spPoint.vector3Value.ToString());

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(spStart);
        EditorGUILayout.PropertyField(spWFE);
        EditorGUILayout.EndHorizontal();

        if (toggle = EditorGUILayout.BeginFoldoutHeaderGroup(toggle, data.ToString()))
        {
            EditorGUILayout.PropertyField(spPoint);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}