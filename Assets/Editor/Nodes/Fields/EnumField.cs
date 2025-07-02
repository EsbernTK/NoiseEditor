using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[System.Serializable]
public class EnumField : AbstractField {

    public new Operators value;
    public EnumField(BaseNode owner, string name, Operators initValue, FieldType fieldType, bool drawField = true) : base(owner, name, fieldType, drawField)
    {
        value = initValue;

    }
    public override void Show()
    {
        value = (Operators)EditorGUILayout.EnumPopup(name, value);
        base.Show();
    }
}
