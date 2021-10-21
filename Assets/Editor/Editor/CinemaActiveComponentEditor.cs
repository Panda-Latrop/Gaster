using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;


    [CustomEditor(typeof(CinemaActiveComponent))]
    public class CinemaActiveComponentEditor : Editor
{
    protected bool toggle;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var spStart = serializedObject.FindProperty("start");
        var spWFE = serializedObject.FindProperty("waitForEnd");
        var spType = serializedObject.FindProperty("type");
        var spActive = serializedObject.FindProperty("active");
        var spTarget = serializedObject.FindProperty("target");

        CinemaActiveComponent.CinemaActiveObjectType type = (CinemaActiveComponent.CinemaActiveObjectType)spType.enumValueIndex;
        StringBuilder data = new StringBuilder("Data: ");
        data.Append(spActive.boolValue? "Activate: " : "Deactivate: ").Append(type.ToString().ToUpper());

        if(type.Equals(CinemaActiveComponent.CinemaActiveObjectType.target))
                data.Append(", ").Append("By: ").Append(spTarget.objectReferenceValue != null ? spTarget.objectReferenceValue.name : "NULL");
                

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(spStart);
        EditorGUILayout.PropertyField(spWFE);
        EditorGUILayout.EndHorizontal();
        if (toggle = EditorGUILayout.BeginFoldoutHeaderGroup(toggle, data.ToString()))
        {
            EditorGUILayout.PropertyField(spActive);
            EditorGUILayout.PropertyField(spType);
            if (type.Equals(CinemaActiveComponent.CinemaActiveObjectType.target))
            {
                EditorGUILayout.PropertyField(spTarget);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}