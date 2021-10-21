using UnityEditor;


[CustomEditor(typeof(ActionActiveComponent))]
public class ActionActiveComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("target"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("active"));
        serializedObject.ApplyModifiedProperties();
    }
}
