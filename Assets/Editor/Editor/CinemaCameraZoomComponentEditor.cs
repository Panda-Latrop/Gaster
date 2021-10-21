using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;


    [CustomEditor(typeof(CinemaCameraZoomComponent))]
    public class CinemaCameraZoomComponentEditor : Editor
{
    protected bool toggle;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var spStart = serializedObject.FindProperty("start");
        var spWFE = serializedObject.FindProperty("waitForEnd");
        var spType = serializedObject.FindProperty("type");
        var spZoom = serializedObject.FindProperty("zoom");

        CinemaCameraZoomComponent.CinemaCameraZoomType type = (CinemaCameraZoomComponent.CinemaCameraZoomType)spType.enumValueIndex;
        StringBuilder data = new StringBuilder("Data: ");
        data.Append(type.ToString().ToUpper());
        switch (type)
        {
            case CinemaCameraZoomComponent.CinemaCameraZoomType.to:
                data.Append(", ").Append(spZoom.floatValue);
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
                case CinemaCameraZoomComponent.CinemaCameraZoomType.to:
                    EditorGUILayout.PropertyField(spZoom);
                    break;
                default:
                    break;
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}