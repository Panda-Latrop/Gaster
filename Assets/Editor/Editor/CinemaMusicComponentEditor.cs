using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CinemaMusicComponent))]
public class CinemaMusicComponentEditor : Editor
{
    protected bool toggle;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var spStart = serializedObject.FindProperty("start");
        var spWFE = serializedObject.FindProperty("waitForEnd");
        var spType = serializedObject.FindProperty("type");
        var spMusic = serializedObject.FindProperty("music");

        CinemaMusicComponent.CinemaMusicType type = (CinemaMusicComponent.CinemaMusicType)spType.enumValueIndex;
        AudioClip audio = spMusic.objectReferenceValue as AudioClip;
        StringBuilder data = new StringBuilder("Data: ");
        data.Append(type.ToString().ToUpper());
        switch (type)
        {
            case CinemaMusicComponent.CinemaMusicType.change:
                if(audio != null)
                data.Append(", ").Append(audio.name);
                else
                    data.Append(", ").Append("NONE");
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
                case CinemaMusicComponent.CinemaMusicType.change:
                    EditorGUILayout.PropertyField(spMusic);
                    break;
                default:
                    break;
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}