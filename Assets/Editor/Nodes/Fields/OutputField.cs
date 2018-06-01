using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OutputField : AbstractField {
    Color fieldColor;
    public OutputField(BaseNode owner, Color fieldColor, string name = "Output") : base(owner,name,FieldType.Output)
    {
        this.fieldColor = fieldColor;
    }
    public override void Show()
    {
        GUILayout.Space(15);
        GUILayout.Label(name);
        base.Show();
    }
}
