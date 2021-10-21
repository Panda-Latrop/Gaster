using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CinemaParticleComponent))]
public class CinemaParticleComponentEditor : Editor
{
    protected bool toggle;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var spStart = serializedObject.FindProperty("start");
        var spWFE = serializedObject.FindProperty("waitForEnd");
        var spType = serializedObject.FindProperty("type");
        var spParticle = serializedObject.FindProperty("particle");

        CinemaParticleComponent.CinemaParticleType type = (CinemaParticleComponent.CinemaParticleType)spType.enumValueIndex;
        ParticleSystem particle = spParticle.objectReferenceValue as ParticleSystem;
        StringBuilder data = new StringBuilder("Data: ");
        data.Append(type.ToString().ToUpper());

        if (particle != null)
            data.Append(", ").Append(particle.name);
        else
            data.Append(", ").Append("NONE");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(spStart);
        EditorGUILayout.PropertyField(spWFE);
        EditorGUILayout.EndHorizontal();
        if (toggle = EditorGUILayout.BeginFoldoutHeaderGroup(toggle, data.ToString()))
        {

            EditorGUILayout.PropertyField(spParticle);
            EditorGUILayout.PropertyField(spType);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}