using UnityEngine;
using UnityEditor;
[System.Serializable]
public class NodeField : AbstractField {

    public new BaseNode value;
    public NodeField(BaseNode owner, string name = "Node", FieldType type = FieldType.Input, IsFieldShaderVariable isShaderVariable  = IsFieldShaderVariable.Yes) : base(owner, name, type,true,isShaderVariable)
    {

    }
    public override void Show()
    {
        if(value != null)
            GUILayout.Label(name + "   " + value.windowName);
        else
            GUILayout.Label(name);
        base.Show();

    }
}
