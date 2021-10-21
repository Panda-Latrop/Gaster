using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CinemaAnimationComponent))]
public class CinemaAnimationComponentEditor : Editor
{
    protected bool toggle;
    public override void OnInspectorGUI()
    {

        serializedObject.Update();
        var spStart = serializedObject.FindProperty("start");
        var spWFE = serializedObject.FindProperty("waitForEnd");
        var spAnr = serializedObject.FindProperty("animator");
        var spAnm = serializedObject.FindProperty("animation");
        var spCro = serializedObject.FindProperty("crossFade");
        var sptd = serializedObject.FindProperty("transitionDuration");

        bool cross = spCro.boolValue;
        bool hasAnimator = spAnr.objectReferenceValue != null;
        Transform transform = default;
        if (hasAnimator)
            transform = (spAnr.objectReferenceValue as Animator).transform;

        StringBuilder data = new StringBuilder("Data: ");

        if (hasAnimator)
        {
            data.Append(transform.parent.name).Append(", Play: ").Append(spAnm.stringValue);
        }
        else
        {
            data.Append("NULL");
        }
      
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(spStart);
        EditorGUILayout.PropertyField(spWFE);
        EditorGUILayout.EndHorizontal();


        if (toggle = EditorGUILayout.BeginFoldoutHeaderGroup(toggle, data.ToString()))
        {
            EditorGUILayout.PropertyField(spAnr);
            if (hasAnimator)
            {
                EditorGUILayout.PropertyField(spAnm);
                EditorGUILayout.PropertyField(spCro);
                if(cross)
                    EditorGUILayout.PropertyField(sptd);
            }         
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}
