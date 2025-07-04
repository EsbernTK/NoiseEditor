﻿using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Vector3IntField : AbstractField
{
    public new Vector3Int value;
    bool foldedOut;
    public Vector3IntField(BaseNode owner, string name, Vector3Int initValue, FieldType fieldType, bool drawField = true, IsFieldShaderVariable isShaderVariable = IsFieldShaderVariable.Yes) : base(owner,name, fieldType, drawField,isShaderVariable)
    {

        value = initValue;

    }
    public override void Show(Vector2 position)
    {

        Rect posSize = new Rect(position, new Vector2(50, 50));
        if (fieldType != FieldType.Static)
            value = EditorGUI.Vector3IntField(posSize, name, value);
        else
            EditorGUI.Vector3Field(posSize, name, value);
    }
    public override void Show()
    {

        if (fieldType != FieldType.Static && fieldType != FieldType.Input)
            value = EditorGUILayout.Vector3IntField(name, value);
        else if (fieldType == FieldType.Input)
        {
            foldedOut = EditorGUILayout.Foldout(foldedOut, name);
            if (!foldedOut)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                value = EditorGUILayout.Vector3IntField("", value, GUILayout.MaxWidth(200f));
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
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                EditorGUILayout.LabelField("Z", GUILayout.MaxWidth(11f));
                value.z = EditorGUILayout.DelayedIntField(value.z, GUILayout.MaxWidth(50f));
                GUILayout.EndHorizontal();
            }


        }
        else
        {
            EditorGUILayout.Vector3Field(name, value);
        }
        base.Show();
    }
}