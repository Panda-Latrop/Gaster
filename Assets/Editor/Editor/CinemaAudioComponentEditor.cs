using System.Text;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CinemaAudioComponent))]
    public class CinemaAudioComponentEditor : Editor
{
    protected bool toggle;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var spStart = serializedObject.FindProperty("start");
        var spWFE = serializedObject.FindProperty("waitForEnd");
        var spType = serializedObject.FindProperty("type");
        var spSource = serializedObject.FindProperty("source");
        var spSound = serializedObject.FindProperty("sound");

        CinemaAudioComponent.CinemaAudioType type = (CinemaAudioComponent.CinemaAudioType)spType.enumValueIndex;
        AudioSource source = spSource.objectReferenceValue as AudioSource;
        AudioClip audio = spSound.objectReferenceValue as AudioClip;
        StringBuilder data = new StringBuilder("Data: ");
        data.Append(type.ToString().ToUpper());
        switch (type)
        {
            case CinemaAudioComponent.CinemaAudioType.play:

                if (source != null && audio != null)
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
            
            EditorGUILayout.PropertyField(spSource);
            EditorGUILayout.PropertyField(spType);
            switch (type)
            {
                case CinemaAudioComponent.CinemaAudioType.play:
                    EditorGUILayout.PropertyField(spSound);
                    break;
                default:
                    break;
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}