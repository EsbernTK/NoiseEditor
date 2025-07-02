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
        base.value = value;
        
    }
    public override void Show()
    {

        if (fieldType != FieldType.Static)
            value = EditorGUILayout.DelayedTextField(name, value);
        
        else
        {
            EditorGUILayout.LabelField(name, value);
        }
        base.value = value;
        if (owner != null)
            owner.windowName = value;
        //Debug.Log("StringField Show called for " + name + " with value: " + value);
        base.Show();
    }
    
}
