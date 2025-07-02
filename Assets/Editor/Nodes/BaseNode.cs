using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
[System.Serializable]
public class BaseNode : ScriptableObject, ISerializationCallbackReceiver {
    //[SerializeReference]
    [SerializeField]
    public List<AbstractField> fields = new List<AbstractField>();
    public Dictionary<string, AbstractField> fieldDict = new Dictionary<string, AbstractField>();
    public Rect windowRect;
    public string windowName;
    public float offsetWidth = 10;
    public float offsetHeight = 10;
    public Rect outputCirc;
    public int id;
    public float recordWidth;
    [SerializeField]
    public List<List<BaseNode>> inputNodes = new List<List<BaseNode>>();
    [SerializeField]
    public List<BaseNode> outputNodes = new List<BaseNode>();
    public string baseVariableName = "";
    public BaseNode()
    {
        //windowRect = new Rect(0, 0, 215, 215);
        //EditorUtility.SetDirty(this);

    }
    
    
    public virtual void ShowNode()
    {
        foreach (AbstractField field in fields)
        {

            if (field.drawField)
                field.Show();
        }
        if (Event.current.type == EventType.Repaint)
        {
            if (this is IHasOutput)
                outputCirc = fields[fields.Count - 1].inputCirc;
            /*
            recordWidth = 0;
            foreach(AbstractField field in fields)
            {
                if (field.fieldRect.width > recordWidth)
                    recordWidth = field.fieldRect.width;
            }*/
            /*
            Rect tempRect = GUILayoutUtility.GetLastRect();
            outputCirc = new Rect(tempRect.x + tempRect.width - 5, tempRect.y + tempRect.height + 10, 10, 10);
            Handles.color = Color.black;
            Handles.DrawSolidArc(outputCirc.position, Vector3.forward, Vector3.left, 360f, outputCirc.width / 2);
            */
            /*
            windowRect.height = outputCirc.y + outputCirc.height + offsetHeight;
            windowRect.width = recordWidth + offsetWidth;
            Debug.Log(recordWidth);
            */
        }
    }
    public virtual void AddField(AbstractField field)
    {
        inputNodes.Add(new List<BaseNode>());
        field.arrayIndex = inputNodes.Count - 1;
        fields.Add(field);
        if (fieldDict.ContainsKey(field.name))
            fieldDict[field.name] = field;
        else
            fieldDict.Add(field.name, field);
        field.id = fields.Count - 1;
    }
    public virtual void GetTreeStructure(OutputNode baseNode, int depth)
    {
        Debug.Log(windowName);
        List<NodeTreeInfo> curTree = baseNode.tree.Where(x => x.node == this).ToList();
        if(curTree.Count > 0 && curTree[0].depth < depth)
        {
            int ind = baseNode.tree.IndexOf(curTree[0]);
            baseNode.tree[ind] = new NodeTreeInfo
            {
                node = this,
                depth = depth
            };
        }
        else
        {
            baseNode.tree.Add(new NodeTreeInfo
            {
                node = this,
                depth = depth
            });
        }
        foreach(List<BaseNode> l in inputNodes)
        {
            foreach (BaseNode n in l)
            {
                n.GetTreeStructure(baseNode, depth + 1);
            }
        }
    }
    public virtual void AddInput(BaseNode node, Vector2 mousePos)
    {
        AbstractField clickedField = ClickedOnField(mousePos);

        if (clickedField != null)
        {
            bool temp = clickedField.AddInputNode(node);
            SetDirty();
        }
        
    }
    public virtual void RemoveOutput(BaseNode node)
    {
        outputNodes.Remove(node);
        SetDirty();
    }

    public virtual void AddOutput(BaseNode node)
    {
        outputNodes.Add(node);
        SetDirty();
    }

    public virtual void RemoveInput(BaseNode node)
    {
        foreach (AbstractField f in fields)
        {
            f.RemoveInputNode(node);
        }
        SetDirty();
    }
    
    public virtual void SetDirty()
    {
        EditorUtility.SetDirty(this);
    }

    public virtual void DeleteNode()
    {
        foreach (List<BaseNode> l in inputNodes)
        {
            foreach (BaseNode n in l)
            {
                n.RemoveOutput(this);
            }
        }
        foreach (BaseNode n in outputNodes)
        {
            n.RemoveInput(this);
        }
        DestroyImmediate(this);
    }
    public virtual AbstractField ClickedOnField(Vector2 clickPos)
    {
        foreach(AbstractField f in fields)
        {
            AbstractField tempField = f.ClickedOnField(clickPos - windowRect.position);
            if(tempField != null) {
                return tempField;
            }
        }
        return null;
    }


    public virtual void OnBeforeSerialize()
    {
        //Serialize the fields list before serialization
        for (int i = 0; i < fields.Count; i++)
        {
            if (fields[i] != null)
            {
                //serializedFields.Add(fields[i].SerializeField());
            }
            else
            {
                //serializedFields.Add("");
            }
        }



    }

    public virtual void OnAfterDeserialize()
    {
        // This method is called after deserialization
        // You can use it to restore any transient state or perform post-deserialization actions
        fieldDict.Clear();
        for (int i = 0; i < fields.Count; i++)
        {
            fields[i].owner = this;
            fields[i].id = i;
            if (!fieldDict.ContainsKey(fields[i].name))
                fieldDict.Add(fields[i].name, fields[i]);
            else
                fieldDict[fields[i].name] = fields[i];
        }
    }


    public void ShowConnections()
    {
        for (int i = 0; i < inputNodes.Count; i++)
        {
            foreach (BaseNode n in inputNodes[i])
            {
                Rect start = new Rect(n.outputCirc.center + n.windowRect.position, Vector2.zero);
                Rect destination = new Rect(fields[i].inputCirc.center + windowRect.position, Vector2.zero);
                NoiseEditorWindow.DrawNodeCurve(start, destination);
            }
        }
    }

    public void ShowConnectionToCourser(Vector2 mousePos)
    {
        Rect start = new Rect(this.outputCirc.center + this.windowRect.position, Vector2.zero);
        Rect destination = new Rect(mousePos, Vector2.zero);
        NoiseEditorWindow.DrawNodeCurve(start, destination);
    }

    public bool IsInfiniteConnection(BaseNode node)
    {
        if (Equals(node))
        {
            return true;
        }
        if (outputNodes.Count > 0)
        {
            foreach(BaseNode n in outputNodes)
            {
                if (n.IsInfiniteConnection(node))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public string GetVariableDeclarations(string givenNodeName)
    {
        string variableDeclarationBlock = "";
        foreach(AbstractField f in fields)
        {
            if (f.isShaderVariable == IsFieldShaderVariable.Yes)
            {
                string tempVariable = f.GetVariableDeclaration(givenNodeName);
                variableDeclarationBlock += tempVariable + "\n    ";
            }
        }
        Debug.Log(variableDeclarationBlock);
        return variableDeclarationBlock;
    }
    public virtual string GetFunctionCall(string givenNodeName, List<NodeTreeInfo> tree)
    {
        return "";
    }
    public virtual string GetBaseFunctionDeclarations()
    {
        return "";
    }
    public virtual string GetMainFunctionDeclarations()
    {
        return "";
    }
}
