using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CinemaCameraMovementComponent))]

public class CinemaCameraMovementComponentEditor : Editor
{
    protected bool toggle;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var spStart = serializedObject.FindProperty("start");
        var spWFE = serializedObject.FindProperty("waitForEnd");
        var spType = serializedObject.FindProperty("type");
        var spSM = serializedObject.FindProperty("speedMultiply");
        var spPoint = serializedObject.FindProperty("point");
        var spPath = serializedObject.FindProperty("path");

        

        CinemaCameraMovementComponent.CinemaCameraMovementType type = (CinemaCameraMovementComponent.CinemaCameraMovementType)spType.enumValueIndex;
        StringBuilder data = new StringBuilder("Data: ");
        data.Append(type.ToString().ToUpper());
        switch (type)
        {
            case CinemaCameraMovementComponent.CinemaCameraMovementType.teleport:
            case CinemaCameraMovementComponent.CinemaCameraMovementType.moveToPoint:
                data.Append(", ").Append("To: ").Append(spPoint.vector3Value.ToString());
                break;
            case CinemaCameraMovementComponent.CinemaCameraMovementType.moveByPath:
                data.Append(", ").Append("By: ").Append(spPath.objectReferenceValue != null? spPath.objectReferenceValue.name:"NULL");
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
                case CinemaCameraMovementComponent.CinemaCameraMovementType.teleport:
                    EditorGUILayout.PropertyField(spPoint);
                    break;
                case CinemaCameraMovementComponent.CinemaCameraMovementType.moveToPoint:
                    EditorGUILayout.PropertyField(spSM); 
                    EditorGUILayout.PropertyField(spPoint);
                    break;
                case CinemaCameraMovementComponent.CinemaCameraMovementType.moveByPath:
                    EditorGUILayout.PropertyField(spSM);
                    EditorGUILayout.PropertyField(spPath);
                    break;
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}
