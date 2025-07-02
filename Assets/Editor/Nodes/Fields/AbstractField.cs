using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class AbstractField: IField{
    public string name;
    public FieldType fieldType;
    public IsFieldShaderVariable isShaderVariable;
    [SerializeReference] public BaseNode owner;
    public Rect fieldRect;
    public bool drawField;
    public int id;
    public int arrayIndex;
    public virtual object value { get; set; }
    public Rect inputCirc;
    public BaseNode inputNode;
    public AbstractField(BaseNode owner,string name = "Place holder field", FieldType fieldType = FieldType.NoInput, bool drawField = true, IsFieldShaderVariable isShaderVariable= IsFieldShaderVariable.No)
    {
        this.fieldType = fieldType;
        this.isShaderVariable = isShaderVariable;
        this.name = name;
        this.drawField = drawField;
        this.owner = owner;
    }
    public virtual void Show(Vector2 position)
    {

    }
    public virtual void Show()
    {
        EventType e = Event.current.type;
        if (e == EventType.Repaint)
        {
            
            fieldRect = GUILayoutUtility.GetLastRect();
            //Debug.Log(name + "" + fieldRect);
            if (fieldType == FieldType.Input)
            {
                inputCirc = new Rect(fieldRect.x - 8, fieldRect.y + fieldRect.height/4-2, 10, 10);
                Handles.color = Color.black;
                Handles.DrawSolidArc(inputCirc.center, Vector3.forward, Vector3.left, 360f, inputCirc.width / 2);
            }
            //Debug.Log(fieldRect);
            if (fieldType == FieldType.Output)
            {
                inputCirc = new Rect(fieldRect.x + fieldRect.width - 10, fieldRect.y + fieldRect.height / 2, 10, 10);
                Handles.color = Color.black;
                Handles.DrawSolidArc(inputCirc.center, Vector3.forward, Vector3.left, 360f, inputCirc.width / 2);
            }
        }

    }
    public virtual bool AddInputNode(BaseNode newInputNode)
    {
        if(fieldType == FieldType.Input)
        {
            if(inputNode != null)
            {
                inputNode.RemoveOutput(owner);
                owner.inputNodes[arrayIndex].Remove(inputNode);
            }
            inputNode = newInputNode;
            owner.inputNodes[arrayIndex].Add(inputNode);
        }
        return fieldType == FieldType.Input;
    }
    public virtual void RemoveInputNode(BaseNode node)
    {
        if (node == inputNode)
        {
            owner.inputNodes[arrayIndex].Remove(inputNode);
            inputNode = null;
        }
    }
    public AbstractField ClickedOnField(Vector2 mousePos)
    {   
        //Debug.Log("Clicked on field: " + name + " at " + mousePos + " with fieldType: " + fieldType + " and inputCirc: " + inputCirc);
        if (fieldType == FieldType.Input || fieldType == FieldType.Output)
        {
            //Debug.Log(owner.windowName + "   " + name + "     " + inputCirc + "     " + mousePos + "     " + inputCirc);
            if (inputCirc.Contains(mousePos))
            {
                //Debug.Log("Clicked " + name);
                return this;
            }
        }
        return null;
    }


    public virtual string SerializeField()
    {
        //Use the built-in serialization to convert the field to a string
        string serialized = this.ToString();

        return name + ":" + fieldType.ToString() + ":" + isShaderVariable.ToString() + ":" + value?.ToString();
    }

    public virtual void DeserializeField(string serializedField)
    {
        string[] parts = serializedField.Split(':');
        if (parts.Length >= 4)
        {
            name = parts[0];
            fieldType = (FieldType)Enum.Parse(typeof(FieldType), parts[1]);
            isShaderVariable = (IsFieldShaderVariable)Enum.Parse(typeof(IsFieldShaderVariable), parts[2]);
            if (parts[3] != "null")
            {
                value = Convert.ChangeType(parts[3], Type.GetType(parts[3].GetType().FullName));
            }
        }
    }


    public string GetVariableDeclaration(string givenName)
    {
        if (this.value != null)
        {
            givenName += name.Replace(" ", "");
            string varName = GetVariableTypeString(value) + " ";
            varName += givenName + " = " + GetOutputFormat() + ";";
            //Debug.Log(varName);
            return varName;
        }
        return "";
    }
    public virtual string GetOutputFormat()
    {
        return this.value.ToString();
    }

    public string GetVariableTypeString(object variable)
    {

        var @switch = new Dictionary<Type, string> {
                    { typeof(int), "int"},
                    { typeof(Vector2Int), "int2"},
                    { typeof(Vector3Int), "int3"},
                    { typeof(float), "float" },
                    { typeof(Vector2), "float2"},
                    { typeof(Vector3), "float3"},
        };
        if(@switch.ContainsKey(variable.GetType()))
            return @switch[variable.GetType()];
        return "";
    }
}
