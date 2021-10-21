using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionDialogComponent))]
public class ActionDialogComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("waitForEnd"));
        SerializedProperty spType = serializedObject.FindProperty("type");
        EditorGUILayout.PropertyField(spType);
        switch (spType.enumValueIndex)
        {
            case 0:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("file"));
                break;
            case 1:
                SerializedProperty spUT = serializedObject.FindProperty("useTime");
                EditorGUILayout.PropertyField(spUT);
                if (spUT.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("timeToSkip"));
                }
                break;
            default:              
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
