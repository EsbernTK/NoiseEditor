using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class NoiseEditorWindow : EditorWindow {
    public static NoiseEditorWindow editor;
    public NoiseWindowSaver saver;

    private Vector2 mousePos;

    List<BaseNode> nodes = new List<BaseNode>();
    public string pathName = "Assets/Noise Assets/";
    public string saverName = "NoiseWindow";
    public bool makingConnection;
    private void Awake()
    {
        pathName = pathName + saverName + ".asset";
        try
        {
            
            saver = (NoiseWindowSaver)AssetDatabase.LoadAssetAtPath(pathName, typeof(NoiseWindowSaver));
            Debug.Log("Loading Asset");
            nodes = saver.nodes;
        }
        catch { }
        if (!saver)
        {
            Debug.Log("creating asset");
            saver = new NoiseWindowSaver();
            saver.name = saverName;
            saver.nodes = new List<BaseNode>();
            nodes.Add((NoiseNode)ScriptableObject.CreateInstance("NoiseNode"));
            nodes.Add((NoiseNode)ScriptableObject.CreateInstance("NoiseNode"));
            AssetDatabase.CreateAsset(saver, pathName);
        }
        
    }
    [MenuItem("Window/Noise Editor")]
    static void ShowEditor()
    {
        editor = GetWindow<NoiseEditorWindow>();
    }
    private void OnGUI()
    {
        Event e = Event.current;
        mousePos = e.mousePosition;
        if(e.type == EventType.MouseDown)
        {
            BaseNode tempNode = ClickedOnANode();
            if (e.button == 1)
            {
                
                if (!tempNode)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Add Noise Node"), false, AddNode, typeof(NoiseNode));
                    menu.AddItem(new GUIContent("Add Vector2 Node"), false, AddNode, typeof(Vector2Node));
                    menu.ShowAsContext();
                    e.Use();
                }
                else
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete Node"), false, DeleteNode, tempNode);
                    menu.ShowAsContext();
                    e.Use();
                }
            }
            if(e.button == 0)
            {
                AbstractField tempField = tempNode.ClickedOnField();
                if (tempField != null)
                {
                    
                }
            }
        }

        BeginWindows();
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].windowRect = GUILayout.Window(i, nodes[i].windowRect, showNode, nodes[i].windowName);
        }
        EndWindows();
    }

    private void showNode(int id)
    {
        nodes[id].id = id;
        nodes[id].ShowNode();
        GUI.DragWindow();
        
    }
    public void OnDestroy()
    {
        saver.nodes = nodes;
        AssetDatabase.SaveAssets();
    }
    private void AddNode(object type)
    {
        
        System.Type clb = (System.Type) type;
        ConstructorInfo constructor = clb.GetConstructor(System.Type.EmptyTypes);
        BaseNode node = null;
        try
        {
            node = (BaseNode)constructor.Invoke(null);
            node.windowRect.position = mousePos;
            nodes.Add(node);
        }
        catch
        {
            Debug.Log("Warning: node type must inherit from BaseNode");
        }
        
    }
    private void DeleteNode(object node)
    {
        BaseNode curNode = (BaseNode)node;
        nodes.Remove(curNode);
    }
    private BaseNode ClickedOnANode()
    {
        foreach (BaseNode node in nodes)
        {
            if (node.windowRect.Contains(mousePos))
            {
                return node;
            }

        }
        return null;
    }
    
    public static void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);
        for (int i = 0; i < 3; i++)
        {
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        }
        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
    }
}
