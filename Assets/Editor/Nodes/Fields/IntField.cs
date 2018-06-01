using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IntField : AbstractField
{
    public new int value;
    public IntField(BaseNode owner, string name, int initValue, FieldType fieldType, bool drawField = true) : base(owner,name, fieldType, drawField)
    {
        value = initValue;

    }
    public override void Show()
    {

        if (fieldType != FieldType.Static && fieldType != FieldType.Input)
            value = EditorGUILayout.DelayedIntField(name, value);
        else if (fieldType == FieldType.Input)
        {
            value = EditorGUILayout.DelayedIntField(name, value);
        }
        else
        {
            EditorGUILayout.DelayedIntField(name, value);
        }
        base.Show();
    }
}
