using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
[System.Serializable]
public class OutputNode : BaseNode, IHasInput {

    public List<NodeTreeInfo> tree = new List<NodeTreeInfo>();
    public OutputNode()
    {
        
        windowName = "Output Node";
        AddField(new StringField(this, "Window Name",windowName, FieldType.NoInput));
        AddField(new NodeField(this));
        AddField(new ButtonField(this, "Generate Tree", GetTreeStructure));
    }
    public void GetTreeStructure()
    {
        GetTreeStructure(this, 0);
    }
    public override void GetTreeStructure(OutputNode baseNode, int depth)
    {
        tree = new List<NodeTreeInfo>();
        foreach (List<BaseNode> l in inputNodes)
        {
            foreach (BaseNode n in l)
            {
                n.GetTreeStructure(this, 0);
            }
        }
        tree = tree.OrderByDescending(x => x.depth).ToList();
        for (int i = 0; i < tree.Count; i++)
        {
            string tempName = tree[i].node.windowName.Replace(" ", "").Replace("/", "").Replace(".", "").Replace(",", "");
            tree[i] = new NodeTreeInfo
            {
                node = tree[i].node,
                depth = tree[i].depth,
                varName = tempName +"Nr"+ i
            };
        }
        if(NoiseEditorWindow.editor.shaderCreator == null)
        {
            NoiseEditorWindow.editor.shaderCreator = new ShaderCreator();
        }
        List<NodeTreeInfo> sortedTree = tree.OrderByDescending(x => x.depth).ToList();
        NoiseEditorWindow.editor.shaderCreator.CreateShader(sortedTree);
    }
}
public struct NodeTreeInfo
{
    public BaseNode node;
    public int depth;
    public string varName;
}