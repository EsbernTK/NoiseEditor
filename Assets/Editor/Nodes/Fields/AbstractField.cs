using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public abstract class AbstractField: IField {
    public string name;
    public FieldType fieldType;
    public BaseNode owner;
    public Rect fieldRect;
    public object value;
    public bool drawField;
    public int id;
    public Rect inputCirc;
    public BaseNode inputNode;
    public AbstractField(BaseNode owner,string name = "Place holder field", FieldType fieldType = FieldType.NoInput, bool drawField = true)
    {
        this.fieldType = fieldType;
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
                inputCirc = new Rect(fieldRect.x - 5, fieldRect.y + fieldRect.height / 2, 10, 10);
                Handles.color = Color.black;
                Handles.DrawSolidArc(inputCirc.position, Vector3.forward, Vector3.left, 360f, inputCirc.width / 2);
            }
            //Debug.Log(fieldRect);
            if (fieldType == FieldType.Output)
            {
                inputCirc = new Rect(fieldRect.x + fieldRect.width - 10, fieldRect.y + fieldRect.height / 2, 10, 10);
                Handles.color = Color.black;
                Handles.DrawSolidArc(inputCirc.position, Vector3.forward, Vector3.left, 360f, inputCirc.width / 2);
            }
        }
    }
}
