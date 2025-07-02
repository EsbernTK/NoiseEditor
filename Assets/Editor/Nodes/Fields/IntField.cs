using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class IntField : AbstractField
{
    public new int value;
    public IntField(BaseNode owner, string name, int initValue, FieldType fieldType, bool drawField = true, IsFieldShaderVariable isShaderVariable = IsFieldShaderVariable.Yes) : base(owner,name, fieldType, drawField,isShaderVariable)
    {
        value = initValue;
        base.value = value;
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
        base.value = value;
        base.Show();
    }
}
