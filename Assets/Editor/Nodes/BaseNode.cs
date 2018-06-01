using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class BaseNode : ScriptableObject {
    [SerializeField]
    public List<AbstractField> fields = new List<AbstractField>();
    public Dictionary<string, AbstractField> fieldDict = new Dictionary<string, AbstractField>();
    public Rect windowRect;
    public string windowName;
    public float offsetWidth = 10;
    public float offsetHeight = 10;
    public Rect outputCirc;
    public int id;
    public float recordWidth;
    public BaseNode()
    {
        //windowRect = new Rect(0, 0, 215, 215);
        //EditorUtility.SetDirty(this);
    }
    
    public virtual void ShowNode()
    {
        foreach(AbstractField field in fields)
        {
            
            if(field.drawField)
                field.Show();
        }
        if (Event.current.type == EventType.Repaint)
        {
            if(this is IHasOutput)
                outputCirc = fields[fields.Count - 1].inputCirc;
            /*
            recordWidth = 0;
            foreach(AbstractField field in fields)
            {
                if (field.fieldRect.width > recordWidth)
                    recordWidth = field.fieldRect.width;
            }*/
            /*
            Rect tempRect = GUILayoutUtility.GetLastRect();
            outputCirc = new Rect(tempRect.x + tempRect.width - 5, tempRect.y + tempRect.height + 10, 10, 10);
            Handles.color = Color.black;
            Handles.DrawSolidArc(outputCirc.position, Vector3.forward, Vector3.left, 360f, outputCirc.width / 2);
            */
            /*
            windowRect.height = outputCirc.y + outputCirc.height + offsetHeight;
            windowRect.width = recordWidth + offsetWidth;
            Debug.Log(recordWidth);
            */
        }
    }
    public virtual void AddField(AbstractField field)
    {
        fields.Add(field);
        if (fieldDict.ContainsKey(field.name))
            fieldDict[field.name] = field;
        else
            fieldDict.Add(field.name, field);
        field.id = fields.Count - 1;
    }
    public virtual AbstractField ClickedOnField()
    {
        return null;
    }
}
