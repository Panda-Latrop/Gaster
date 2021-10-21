using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;





public class CinemaEditorNode
{
    public static Vector2 size = new Vector2(200.0f, 75.0f);
    public CinemaBaseComponent component;

    public int line;
    protected Rect rect;
    protected GUIStyle style;
    protected Vector2 scrollPos;
    //private Editor editor;

    public CinemaEditorNode(CinemaBaseComponent component)
    {
        this.component = component;
        rect = new Rect(0.0f, 0.0f, size.x, size.y);
        style = new GUIStyle("sv_iconselector_back");
        //editor = Editor.CreateEditor(this.cinemaNode);
    }

    public void Draw(Rect parent,int line)
    {
        this.line = line;
        rect.position = new Vector2(parent.position.x + component.start* size.x, line * size.y);
        GUILayout.BeginArea(rect, style);
        EditorGUIUtility.fieldWidth = 16;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.LabelField(component.GetType().Name);
        EditorGUILayout.BeginHorizontal();
        component.start = EditorGUILayout.FloatField(component.start);
        //component.end = EditorGUILayout.FloatField(component.end);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
       
        EditorGUIUtility.fieldWidth = 0;
        //editor.OnInspectorGUI();
        //GUI.backgroundColor = Color.white;
    }
    public bool Contains(Vector2 mousePosition)
    {
        return rect.Contains(mousePosition);
    }
}


public class CinemaScenarioWindowEditor : EditorWindow
{
    protected bool hasScenario;
    protected CinemaScenarioBase scenario;
    protected Vector2 dataScrollPos = Vector2.zero;
    protected Vector2 timelineScrollPos = Vector2.zero;
    protected Rect groupRect;


    protected List<CinemaEditorNode> nodes = new List<CinemaEditorNode>();

    [MenuItem("Window/Cinema Scenario Editor")]
    private static void OpenWindow()
    {
        CinemaScenarioWindowEditor window = GetWindow<CinemaScenarioWindowEditor>();
        window.titleContent = new GUIContent("Cinema Scenario Editor");
    }

    protected void OnEnable()
    {
        hasScenario = (Selection.gameObjects.Length > 0 && (scenario = Selection.gameObjects[0].GetComponent<CinemaScenarioBase>()) != null);
        if (hasScenario && scenario.Nodes != null)
        {
            for (int i = 0; i < scenario.Nodes.Count; i++)
            {
               nodes.Add(new CinemaEditorNode(scenario.Nodes[i]));
            }
        }        
    }

    void OnGUI()
    {

        if (!hasScenario)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Select Scenario");
            hasScenario = (Selection.gameObjects.Length > 0 && (scenario = Selection.gameObjects[0].GetComponent<CinemaScenarioBase>()) != null);
            if (hasScenario && scenario.Nodes != null)
            {
                for (int i = 0; i < scenario.Nodes.Count; i++)
                {
                    nodes.Add(new CinemaEditorNode(scenario.Nodes[i]));
                }
            }
            return;
        }

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, 
                                          GUILayout.Height(position.height - EditorGUIUtility.standardVerticalSpacing*2),
                                          GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth*0.3f));
            {
                dataScrollPos = EditorGUILayout.BeginScrollView(dataScrollPos);
                {

                    EditorGUILayout.LabelField("LabelField 1");
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            groupRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox, 
                                          GUILayout.Height(position.height- EditorGUIUtility.standardVerticalSpacing*2), 
                                          GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.7f));
            {
               
                    DrawTimeline();                 

            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
       // 


        //scrollPos = GUILayout.BeginScrollView(scrollPos);   
        //GUILayout.EndScrollView();

        Repaint();
    }


    protected void DrawTimeline()
    {
        timelineScrollPos = EditorGUILayout.BeginScrollView(timelineScrollPos);
        {
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw(groupRect,i);
            }
       
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();




        /*Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);
        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, groupRect.height, 0f) + newOffset);
        }
        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(groupRect.width, gridSpacing * j, 0f) + newOffset);
        }
        Handles.color = Color.white;
        Handles.EndGUI();*/
}



}













/*
public class CinemaEditorNode
{
    public static Vector2 size = new Vector2(250.0f, 150.0f);
    public ActionBaseComponent cinemaNode;
    public Rect rect;
    private GUIStyle style;
    private Editor editor;
    private Vector2 scrollPos;

    public CinemaEditorNode(ActionBaseComponent cinemaNode)
    {
        this.cinemaNode = cinemaNode;
        rect = new Rect(0.0f, 0.0f, size.x, size.y);
        style = new GUIStyle("sv_iconselector_back");
        editor = Editor.CreateEditor(this.cinemaNode);
    }

    public void Draw(Vector2 position, Vector2 offset)
    {
        position += offset;
        Draw(position);
    }
    public void Draw(Vector2 position)
    {
        rect.position = position;
        if (cinemaNode.WaitForEnd)
            GUI.backgroundColor = Color.yellow;
        else
            GUI.backgroundColor = Color.cyan;
        
        GUILayout.BeginArea(rect, style);
        EditorGUILayout.LabelField(cinemaNode.GetType().Name);
        EditorGUIUtility.labelWidth = 96;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        editor.OnInspectorGUI();
        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
        EditorGUIUtility.labelWidth = 0;
        GUI.backgroundColor = Color.white;
    }
    public bool Contains(Vector2 mousePosition)
    {
        return rect.Contains(mousePosition);
    }
    /*private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove NodeMy"), false, OnClickRemoveNodeMy);
        genericMenu.ShowAsContext();
    }
}

public class CinemaScenarioWindowEditor : EditorWindow
{
    protected bool hasCinemaScenario;
    protected CinemaScenarioBase cinemaScenario;
    protected List<CinemaEditorNode> nodes = new List<CinemaEditorNode>();

    protected CinemaEditorNode selected, copied,swaped;
    protected bool cuted;

    protected Vector2 offset;
    private float scaling = 1.0f;
    private Vector2 mousePosition;
    private Vector2 scrollPosition;
    private Rect groupRect;
    private const float groupHeaderSize = 21.0f;
    private const float maxGraphSize = 16000.0f;

    protected Type[] types = { 
                                typeof(ActionDelayComponent), 
                                typeof(ActionActiveComponent),
                                typeof(ActionAnimationComponent), 
                                typeof(ActionMovementComponent),
                                typeof(ActionParticleComponent),
                                typeof(ActionSoundComponent), 
                                typeof(ActionCameraMovementComponent),
                                typeof(ActionDialogComponent),
    };

    [MenuItem("Window/Cinema Scenario Editor")]
    private static void OpenWindow()
    {
        CinemaScenarioWindowEditor window = GetWindow<CinemaScenarioWindowEditor>();
        window.titleContent = new GUIContent("Cinema Scenario Editor");


    }
    protected void OnEnable()
    {
        hasCinemaScenario = (Selection.gameObjects.Length > 0 && (cinemaScenario = Selection.gameObjects[0].GetComponent<CinemaScenarioBase>()) != null);
        if (hasCinemaScenario && cinemaScenario.Nodes != null)
        {
            for (int i = 0; i < cinemaScenario.Nodes.Count; i++)
            {
                nodes.Add(new CinemaEditorNode(cinemaScenario.Nodes[i]));
            }
        }
        offset.Set(position.width * 1.5f, position.height / 4.0f);

    }
    private void OnGUI()
    {
        if (cinemaScenario == null)
            return;
        DrawScaleWindow();
        ProcessEvents(Event.current);
        if (GUI.changed)
            Repaint();
    }

    private void DrawScaleWindow()
    {
        GUI.EndGroup();
        groupRect.x = 0;
        groupRect.y = groupHeaderSize;
        groupRect.width = position.width / scaling;
        groupRect.height = position.height / scaling;
        GUI.BeginGroup(groupRect);
        Matrix4x4 old = GUI.matrix;
        Matrix4x4 translation = Matrix4x4.TRS(new Vector3(0, 21, 1), Quaternion.identity, Vector3.one);
        Matrix4x4 scale = Matrix4x4.Scale(new Vector3(scaling, scaling, scaling));
        GUI.matrix = translation * scale * translation.inverse;
        GUILayout.BeginArea(new Rect(0, 0, maxGraphSize * scaling, maxGraphSize * scaling));
        //
        DrawGrid(20, 0.2f, Color.gray);
        DrawNode();
        //
        GUILayout.EndArea();
        GUI.matrix = old;
        GUI.EndGroup();
        GUI.BeginGroup(new Rect(0, groupHeaderSize, position.width, position.height));

    }
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt((position.width / gridSpacing) / scaling);
        int heightDivs = Mathf.CeilToInt((position.height / gridSpacing) / scaling);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);
        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, groupRect.height, 0f) + newOffset);
        }
        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(groupRect.width, gridSpacing * j, 0f) + newOffset);
        }
        Handles.color = Color.white;
        Handles.EndGUI();
    }
    private void DrawNode()
    {
        int column = 0, row = 0;
        Vector2 position = Vector2.zero;
        bool prefWaitForEnd;
        bool waitForEnd;

        if (nodes.Count > 0)
        {

            waitForEnd = nodes[0].cinemaNode.WaitForEnd;
            position.Set(column * (CinemaEditorNode.size.x + 32.0f), row * (CinemaEditorNode.size.y + 32.0f));
            nodes[0].Draw(position, offset);
            column++;
            if (waitForEnd)
            {
                row++;
                column = 0;
            }

            for (int i = 1; i < nodes.Count; i++)
            {
                prefWaitForEnd = nodes[i - 1].cinemaNode.WaitForEnd;
                waitForEnd = nodes[i].cinemaNode.WaitForEnd;
                if (waitForEnd && !prefWaitForEnd)
                {
                    row++;
                    column = 0;
                }
                position.Set(column * (CinemaEditorNode.size.x + 32.0f), row * (CinemaEditorNode.size.y + 32.0f));
                nodes[i].Draw(position, offset);
                column++;
                if (waitForEnd)
                {
                    row++;
                    column = 0;
                }

            }
        }
    }

    private bool SelectNode()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].Contains(mousePosition))
            {
                selected = nodes[i];
                return true;
            }
        }
        return false;
    }


    private void ProcessEvents(Event e)
    {
        mousePosition = (e.mousePosition + scrollPosition) / scaling;
        switch (e.type)
        {

            case EventType.MouseDown:
                
                if (e.button == 0)
                {
                    //ClearConnectionSelection();
                    //EditorGUIUtility.editingTextField = false;
                    e.Use();
                }
                if (e.button == 1 && !e.alt)
                {
                    if (SelectNode())
                    {
                        ProcessNodeContextMenu(mousePosition);
                        e.Use();
                        GUI.changed = true;
                    }
                    else
                    {
                        ProcessContextMenu(mousePosition);
                        e.Use();
                        GUI.changed = true;
                    }

                }
                
                break;
            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    offset += e.delta / scaling;
                    e.Use();
                    GUI.changed = true;
                }
                if (e.button == 1 && !e.control && e.alt)
                {
                    float shiftMultiplier = e.shift ? 4 : 1;
                    scaling = Mathf.Clamp(scaling - e.delta.y * 0.01f * shiftMultiplier, 0.5f, 1f);
                    e.Use();
                    GUI.changed = true;
                }
                break;
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();

        for (int i = 0; i < types.Length; i++)
        {
            Type type = types[i];
            genericMenu.AddItem(new GUIContent("Add Node/" + type.Name), false, () => ProcessContextMenuAddNode(type));
        }
        genericMenu.ShowAsContext();
    }


    private void ProcessContextMenuAddNode(Type type)
    {
        var node = cinemaScenario.gameObject.AddComponent(type) as ActionBaseComponent;
        cinemaScenario.Nodes.Add(node);
        nodes.Add(new CinemaEditorNode(node));
    }
    private void ProcessNodeContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();

        for (int i = 0; i < types.Length; i++)
        {
            Type type = types[i];
            genericMenu.AddItem(new GUIContent("Add Node/From Left/" + type.Name), false, () => ProcessNodeContextMenuAddNode(type, -1));
            genericMenu.AddItem(new GUIContent("Add Node/From Right/" + type.Name), false, () => ProcessNodeContextMenuAddNode(type, 1));
        }
        if(swaped == null)
        {
            genericMenu.AddItem(new GUIContent("Swap From"), false,() => ProcessNodeContextMenuSwapNode(0));
        }
        else
        {
            genericMenu.AddItem(new GUIContent("Swap To"), false, () => ProcessNodeContextMenuSwapNode(1));
        }
        genericMenu.AddItem(new GUIContent("Copy Node"), false, ProcessNodeContextMenuCopyNode);
        genericMenu.AddItem(new GUIContent("Cut Node"), false, ProcessNodeContextMenuCutNode);
        if (copied != null)
        {
            genericMenu.AddItem(new GUIContent("Paste Node/From Left"), false, () => ProcessNodeContextMenuPasteNode(-1));
            genericMenu.AddItem(new GUIContent("Paste Node/From Right"), false, () => ProcessNodeContextMenuPasteNode(1));
        }
        genericMenu.AddItem(new GUIContent("Remove Node"), false, () => ProcessNodeContextMenuRemoveNode(ref selected));
        genericMenu.ShowAsContext();
    }
    private void ProcessNodeContextMenuAddNode(Type type, int side)
    {
        int index = cinemaScenario.Nodes.IndexOf(selected.cinemaNode) + side;
        var node = cinemaScenario.gameObject.AddComponent(type) as ActionBaseComponent;
        if (index < 0)
            index = 0;
        if (index >= cinemaScenario.Nodes.Count)
        {
            cinemaScenario.Nodes.Add(node);
            nodes.Add(new CinemaEditorNode(node));
        }
        else
        {
            cinemaScenario.Nodes.Insert(index, node);
            nodes.Insert(index, new CinemaEditorNode(node));
        }
    }
    private void ProcessNodeContextMenuRemoveNode(ref CinemaEditorNode node)
    {
        cinemaScenario.Nodes.Remove(node.cinemaNode);
        DestroyImmediate(node.cinemaNode);
        nodes.Remove(node);
        if (copied != null && copied.Equals(node))
        {
            copied = null;
            cuted = false;
        }

        node = null;
    }
    private void ProcessNodeContextMenuCopyNode()
    {
        copied = selected;
        cuted = false;
    }
    private void ProcessNodeContextMenuCutNode()
    {
        cuted = true;
        copied = selected;
    }
    private void ProcessNodeContextMenuSwapNode(int state)
    {
        if(state == 0)
        {
            swaped = selected;
        }
        else
        {
            int from = nodes.IndexOf(swaped);
            int to = nodes.IndexOf(selected);
            nodes[from] = selected;
            nodes[to] = swaped;
            selected = swaped = null;
        }
    }
    private void ProcessNodeContextMenuPasteNode(int side)
    {
        int index = cinemaScenario.Nodes.IndexOf(selected.cinemaNode) + side;
        if (index < 0)
            index = 0;
        if (cuted)
        {
            cinemaScenario.Nodes.Remove(copied.cinemaNode);           
            nodes.Remove(copied);
            if (index >= cinemaScenario.Nodes.Count)
            {
                cinemaScenario.Nodes.Add(copied.cinemaNode);
                nodes.Add(copied);
            }
            else
            {
                cinemaScenario.Nodes.Insert(index, copied.cinemaNode);
                nodes.Insert(index, copied);
            }        
        }
        else
        {
            var node = cinemaScenario.gameObject.AddComponent(copied.cinemaNode.GetType()) as ActionBaseComponent;
            if (index >= cinemaScenario.Nodes.Count)
            {
                cinemaScenario.Nodes.Add(node);
                nodes.Add(new CinemaEditorNode(node));
            }
            else
            {
                cinemaScenario.Nodes.Insert(index, node);
                nodes.Insert(index, new CinemaEditorNode(node));
            }

        }
        copied = selected = null;
        cuted = false;
    }
}
*/