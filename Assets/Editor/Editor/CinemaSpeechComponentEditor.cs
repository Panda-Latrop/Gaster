using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CinemaSpeechComponent))]
public class CinemaSpeechComponentEditor : Editor
{
    protected bool toggle;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var spStart = serializedObject.FindProperty("start");
        var spWFE = serializedObject.FindProperty("waitForEnd");
        var spType = serializedObject.FindProperty("type");
        var spFile = serializedObject.FindProperty("file");
        var spTTS = serializedObject.FindProperty("timeToSkip");
        var spHAS = serializedObject.FindProperty("hideAfterSkip");

        CinemaDialogComponent.CinemaDialogType type = (CinemaDialogComponent.CinemaDialogType)spType.enumValueIndex;
        StringBuilder data = new StringBuilder("Data: ");
        data.Append(type.ToString().ToUpper());
        switch (type)
        {
            case CinemaDialogComponent.CinemaDialogType.prepare:
                data.Append(", ").Append("File: ").Append(spFile.stringValue);
                break;
            case CinemaDialogComponent.CinemaDialogType.next:
                    data.Append(", ").Append("Skip: ").Append(spTTS.floatValue);
                    if (spHAS.boolValue)
                        data.Append(", ").Append("Hide");
                break;
            default:
                break;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(spStart);
        EditorGUILayout.PropertyField(spWFE);
        EditorGUILayout.EndHorizontal();
        if (toggle = EditorGUILayout.BeginFoldoutHeaderGroup(toggle, data.ToString()))
        {
            EditorGUILayout.PropertyField(spType);
            switch (type)
            {
                case CinemaDialogComponent.CinemaDialogType.prepare:
                    EditorGUILayout.PropertyField(spFile);
                    break;
                case CinemaDialogComponent.CinemaDialogType.next:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(spTTS);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(spHAS);
                    break;
                default:
                    break;
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}