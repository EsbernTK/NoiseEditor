using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Vector2IntField : AbstractField
{
    public new Vector2Int value;
    bool foldedOut;
    public Vector2IntField(BaseNode owner, string name, Vector2Int initValue, FieldType fieldType, bool drawField = true) : base(owner,name, fieldType, drawField)
    {

        value = initValue;

    }
    public override void Show(Vector2 position)
    {

        Rect posSize = new Rect(position, new Vector2(50, 50));
        if (fieldType != FieldType.Static)
            value = EditorGUI.Vector2IntField(posSize, name, value);
        else
            EditorGUI.Vector2Field(posSize, name, value);
    }
    public override void Show()
    {

        if (fieldType != FieldType.Static && fieldType != FieldType.Input)
            value = EditorGUILayout.Vector2IntField(name, value);
        else if (fieldType == FieldType.Input)
        {
            foldedOut = EditorGUILayout.Foldout(foldedOut, name);
            if (!foldedOut)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                value = EditorGUILayout.Vector2IntField("", value, GUILayout.MaxWidth(200f));
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                EditorGUILayout.LabelField("X", GUILayout.MaxWidth(11f));
                value.x = EditorGUILayout.DelayedIntField(value.x, GUILayout.MaxWidth(50f));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                EditorGUILayout.LabelField("Y", GUILayout.MaxWidth(11f));
                value.y = EditorGUILayout.DelayedIntField(value.y, GUILayout.MaxWidth(50f));
                GUILayout.EndHorizontal();
            }


        }
        else
        {
            EditorGUILayout.Vector2IntField(name, value);
        }
        base.Show();
    }
}


