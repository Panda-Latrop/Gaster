using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CinemaScenarioBase))]
public class CinemaScenarioBaseEditor : Editor
{
    protected CinemaScenarioBase scenario;
    protected CinemaBaseComponent from;
    protected float shift;

    public override void OnInspectorGUI()
    {
        scenario = (CinemaScenarioBase)target;
        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Auto Set"))
        {
            scenario.Nodes.Clear();
            scenario.Nodes.AddRange(scenario.GetComponentsInChildren<CinemaBaseComponent>());
            scenario.duration = scenario.Nodes[scenario.Nodes.Count - 1].start + 0.1f;
            EditorUtility.SetDirty(scenario);
            // scenario.Nodes.Sort();
        }
        if (GUILayout.Button("Clear"))
        {
            scenario.Nodes.Clear();
            EditorUtility.SetDirty(scenario);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Fix Time"))
        {
            float pre = 0;
            for (int i = 0; i < scenario.Nodes.Count; i++)
            {
                if (scenario.Nodes[i].start < pre)
                {
                    scenario.Nodes[i].start = pre + 1;
                }
                pre = scenario.Nodes[i].start;
            }
            scenario.duration = scenario.Nodes[scenario.Nodes.Count - 1].start + 0.1f;
            EditorUtility.SetDirty(scenario);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 32;
        from = EditorGUILayout.ObjectField(from, typeof(CinemaBaseComponent),true) as CinemaBaseComponent;
        shift = EditorGUILayout.FloatField("Shift",shift);
        EditorGUIUtility.labelWidth = 0;
        if (GUILayout.Button("Shift Time") && from != null && scenario.Nodes.Contains(from))
        {
            for (int i = scenario.Nodes.IndexOf(from); i < scenario.Nodes.Count; i++)
            {
                scenario.Nodes[i].start += shift;
            }
            shift = 0;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

    }
}
