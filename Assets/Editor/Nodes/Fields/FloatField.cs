using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FloatField : AbstractField
{
    public new float value;
    public FloatField(BaseNode owner, string name, float initValue, FieldType fieldType, bool drawField = true) : base(owner,name, fieldType, drawField)
    {
        value = initValue;

    }
    public override void Show()
    {

        if (fieldType != FieldType.Static && fieldType != FieldType.Input)
            value = EditorGUILayout.DelayedFloatField(name, value);
        else if (fieldType == FieldType.Input)
        {
            value = EditorGUILayout.DelayedFloatField(name, value);
        }
        else
        {
            EditorGUILayout.DelayedFloatField(name, value);
        }
        base.Show();
    }
}
