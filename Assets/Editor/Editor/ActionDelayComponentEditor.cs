using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(ActionDelayComponent))]
public class ActionDelayComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("waitForEnd"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("time"));
        serializedObject.ApplyModifiedProperties();
    }
}