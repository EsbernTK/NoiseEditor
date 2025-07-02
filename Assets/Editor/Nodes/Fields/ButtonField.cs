using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ButtonField : AbstractField
{
    public new System.Action value;
    public ButtonField(BaseNode owner, string name, System.Action callback, bool drawField = true) : base(owner, name, FieldType.Static, drawField)
    {
        value = callback;

    }
    public override void Show()
    {
        bool temp = GUILayout.Button(name);
        if (temp)
        {
            value();
        }
        base.Show();
    }
}