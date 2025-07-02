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
    public string basePathName = "Assets/Noise Assets/";
    public string pathName = "";
    public string saverName = "NoiseWindow";
    public bool makingConnection;
    BaseNode selectedNode;
    public ShaderCreator shaderCreator = new ShaderCreator();
    private void Awake()
    {
        shaderCreator = new ShaderCreator();

        pathName = basePathName + saverName + ".asset";
        editor = this;
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
        editor = this;
        Event e = Event.current;
        mousePos = e.mousePosition;
        bool graphIsChanged = false;
        if (e.type == EventType.MouseDown)
        {
            BaseNode tempNode = ClickedOnANode();
            if (e.button == 1)
            {
                if (makingConnection)
                {
                    makingConnection = false;
                }
                else if (!tempNode)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Add Noise Node"), false, AddNode, typeof(NoiseNode));
                    menu.AddItem(new GUIContent("Add Arithmatic Node"), false, AddNode, typeof(ArithmaticNode));
                    menu.AddItem(new GUIContent("Add Vector2 Node"), false, AddNode, typeof(Vector2Node));
                    menu.AddItem(new GUIContent("Add Output Node"), false, AddNode, typeof(OutputNode));
                    menu.ShowAsContext();
                    e.Use();
                    graphIsChanged = true;
                }
                else
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete Node"), false, DeleteNode, tempNode);
                    if (tempNode is IHasOutput)
                        menu.AddItem(new GUIContent("Make Connection"), false, MakeConnection, tempNode);
                    menu.ShowAsContext();
                    e.Use();
                    graphIsChanged = true;
                }
            }
            if (e.button == 0)
            {

                if (tempNode && !makingConnection)
                {
                    AbstractField tempField = tempNode.ClickedOnField(e.mousePosition);
                    if (tempField != null)
                    {
                        //Debug.Log("Clicked on field: " + tempField.name);
                        if (tempField.fieldType == FieldType.Input)
                        {
                            //Debug.Log("Clicked on input field with input node: " + tempField.inputNode);
                            if (tempField.inputNode != null)
                            {
                                selectedNode = tempField.inputNode;
                                selectedNode.RemoveOutput(tempNode);
                                tempField.RemoveInputNode(selectedNode);
                                makingConnection = true;
                                graphIsChanged = true;
                            }
                        }
                        else if (tempField.fieldType == FieldType.Output)
                        {
                            selectedNode = tempNode;
                            makingConnection = true;
                            graphIsChanged = true;
                        }
                    }
                }




                else if (makingConnection && tempNode != selectedNode)
                {
                    if (tempNode) //&& tempNode is IHasInput
                    {
                        if (!tempNode.IsInfiniteConnection(selectedNode))
                        {
                            tempNode.AddInput(selectedNode, e.mousePosition);
                            //selectedNode.outputNodes.Add(tempNode);
                            selectedNode.AddOutput(tempNode);
                            makingConnection = false;
                            graphIsChanged = true;
                        }

                    }
                    else makingConnection = false;
                }
            }
        }
        if (makingConnection)
        {
            //DrawNodeCurve(selectedNode.windowRect, new Rect(mousePos, new Vector2(0, 0)));
            selectedNode.ShowConnectionToCourser(mousePos);
            Repaint();
        }
        BeginWindows();
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] == null)
            {
                nodes.RemoveAt(i);
                continue;
            }
            //AbstractField windowField = nodes[i].fieldDict.ContainsKey("Window Name") ? nodes[i].fieldDict["Window Name"] : null;
            AbstractField windowNameField = null;
            if (nodes[i].fieldDict.TryGetValue("Window Name", out windowNameField))
            {
                //Debug.Log(nodes[i] + " has window name field: " + windowNameField.value);
                nodes[i].windowRect = GUILayout.Window(i, nodes[i].windowRect, showNode, windowNameField.value.ToString());
                nodes[i].ShowConnections();
            }
        }
        EndWindows();

        if (graphIsChanged)
        {
            saver.nodes = nodes;
            AssetDatabase.SaveAssets();
            //Debug.Log("Graph changed, saving asset");
        }
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

        /*
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
        */
        string typeName = type.ToString();
        BaseNode node = (BaseNode)ScriptableObject.CreateInstance(typeName);
        node.windowRect.position = mousePos;
        node.id = nodes.Count;
        nodes.Add(node);
        node.SetDirty();
        
    }
    private void MakeConnection(object node)
    {
        BaseNode curNode = (BaseNode)node;
        selectedNode = curNode;
        makingConnection = true;
        
    }
    

    private void DeleteNode(object node)
    {
        BaseNode curNode = (BaseNode)node;
        nodes.Remove(curNode);
        curNode.DeleteNode();
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
