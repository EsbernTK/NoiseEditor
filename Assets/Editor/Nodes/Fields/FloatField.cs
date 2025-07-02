using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class FloatField : AbstractField
{
    public new float value;
    public FloatField(BaseNode owner, string name, float initValue, FieldType fieldType, bool drawField = true, IsFieldShaderVariable isShaderVariable = IsFieldShaderVariable.Yes) : base(owner, name, fieldType, drawField, isShaderVariable)
    {

        value = initValue;
        base.value = value;

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
        base.value = value;
        base.Show();
    }
    

    public override string GetOutputFormat()
    {
        return value.ToString("F4").Replace(" ", "").Replace(",", ".");
    }

}
