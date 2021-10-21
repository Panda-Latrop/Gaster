using UnityEditor;

[CustomEditor(typeof(ActionMovementComponent))]
public class TriggerCollisionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("detector"));
        SerializedProperty spType = serializedObject.FindProperty("type");
        EditorGUILayout.PropertyField(spType);
        switch (spType.enumValueIndex)
        {
            case 0:
                break;
            case 1:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("actor"));
                break;
            default:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("targetTag"));
                break;
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gizmosColor"));       
        serializedObject.ApplyModifiedProperties();
    }
}