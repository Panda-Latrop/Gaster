using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CinemaTransformComponent))]
public class CinemaTransformComponentEditor : Editor
{
    protected bool toggle;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var spStart = serializedObject.FindProperty("start");
        var spWFE = serializedObject.FindProperty("waitForEnd");
        var spTran = serializedObject.FindProperty("transform");
        var spType = serializedObject.FindProperty("type");
        var spAT = serializedObject.FindProperty("actionType");
        var spV = serializedObject.FindProperty("vector");


        bool hasTran = spTran.objectReferenceValue != null;
        GameObject tran = default;
        if (hasTran)
            tran = (spTran.objectReferenceValue as Transform).gameObject;

        CinemaTransformComponent.CinemaTransformType type = (CinemaTransformComponent.CinemaTransformType)spType.enumValueIndex;
        CinemaTransformComponent.CinemaTransformActionType atype = (CinemaTransformComponent.CinemaTransformActionType)spType.enumValueIndex;
        StringBuilder data = new StringBuilder("Data: ");
        if (hasTran)
        {

            data.Append(tran.name).Append(", ").Append(type.ToString().ToUpper());
            data.Append(", ").Append(atype.ToString().ToUpper()).Append(": ");
            switch (atype)
            {
                case CinemaTransformComponent.CinemaTransformActionType.set:
                case CinemaTransformComponent.CinemaTransformActionType.offset:
                    data.Append(spV.vector3Value.ToString());
                    break;
                default:
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
            EditorGUILayout.PropertyField(spTran);
            if (hasTran)
            {
                EditorGUILayout.PropertyField(spType);
                EditorGUILayout.PropertyField(spAT);
                switch (atype)
                {
                    case CinemaTransformComponent.CinemaTransformActionType.set:
                    case CinemaTransformComponent.CinemaTransformActionType.offset:
                        EditorGUILayout.PropertyField(spAT);
                        break;
                    default:
                        break;
                }
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}

