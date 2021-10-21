using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;


    [CustomEditor(typeof(CinemaMovementComponent))]
public class CinemaMovementComponentEditor : Editor
{
    protected bool toggle;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var spStart = serializedObject.FindProperty("start");
        var spWFE = serializedObject.FindProperty("waitForEnd");
        var spMove = serializedObject.FindProperty("movement");
        var spType = serializedObject.FindProperty("type");
        var spSim = serializedObject.FindProperty("simple");
        var spPoint = serializedObject.FindProperty("point");
        var spPath = serializedObject.FindProperty("path");


        bool hasMove = spMove.objectReferenceValue != null;
        GameObject move = default;
        if (hasMove)
            move = (spMove.objectReferenceValue as CharacterMovementComponent).gameObject;

        CinemaCameraMovementComponent.CinemaCameraMovementType type = (CinemaCameraMovementComponent.CinemaCameraMovementType)spType.enumValueIndex;
        StringBuilder data = new StringBuilder("Data: ");
        if (hasMove)
        {

            data.Append(move.name).Append(", ").Append(type.ToString().ToUpper());
            switch (type)
            {
                case CinemaCameraMovementComponent.CinemaCameraMovementType.teleport:
                case CinemaCameraMovementComponent.CinemaCameraMovementType.moveToPoint:
                    data.Append(", ").Append("To: ").Append(spPoint.vector3Value.ToString());
                    break;
                case CinemaCameraMovementComponent.CinemaCameraMovementType.moveByPath:
                    data.Append(", ").Append("By: ").Append(spPath.objectReferenceValue != null ? spPath.objectReferenceValue.name : "NULL");
                    break;
            }
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
            EditorGUILayout.PropertyField(spMove);
            if (hasMove)
            {
                EditorGUILayout.PropertyField(spType);
                switch (type)
                {
                    case CinemaCameraMovementComponent.CinemaCameraMovementType.teleport:
                        EditorGUILayout.PropertyField(spPoint);
                        break;
                    case CinemaCameraMovementComponent.CinemaCameraMovementType.moveToPoint:
                        EditorGUILayout.PropertyField(spPoint);
                        EditorGUILayout.PropertyField(spSim);                 
                        break;
                    case CinemaCameraMovementComponent.CinemaCameraMovementType.moveByPath:
                        EditorGUILayout.PropertyField(spPath);
                        break;
                }
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}