using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class StringField : AbstractField
{
    public new string value;
    public StringField(BaseNode owner, string name, string initValue, FieldType fieldType, bool drawField = true) : base(owner, name, fieldType, drawField)
    {
        value = initValue;

    }
    public override void Show()
    {

        if (fieldType != FieldType.Static)
            value = EditorGUILayout.DelayedTextField(name, value);
        else
        {
            EditorGUILayout.LabelField(name, value);
        }
        base.Show();
    }
    
}
