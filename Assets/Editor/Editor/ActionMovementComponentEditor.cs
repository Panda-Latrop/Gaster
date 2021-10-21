using UnityEditor;

[CustomEditor(typeof(ActionMovementComponent))]
public class ActionMovementComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("waitForEnd"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("character"));
        SerializedProperty spType = serializedObject.FindProperty("type");
        EditorGUILayout.PropertyField(spType);
        switch (spType.enumValueIndex)
        {
            case 0:
            case 1:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("point"));
                break;
            default:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("path"));
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
