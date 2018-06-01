using UnityEngine;
using UnityEditor;
public class NodeField : AbstractField {

    public new BaseNode value;
    public NodeField(BaseNode owner, string name = "Node", FieldType type = FieldType.Input) : base(owner, name, type)
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
