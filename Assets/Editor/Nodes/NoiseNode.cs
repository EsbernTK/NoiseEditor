using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class NoiseNode : BaseNode, IHasFunction, IHasOutput {
    public NoiseNode()
    {
        windowName = "NoiseNode";
        AddField(new StringField(this,"Window Name", windowName, FieldType.NoInput));
        AddField(new Vector3Field(this, "frequency", Vector3.zero, FieldType.Input));
        AddField(new Vector2Field(this, "Octaves", Vector2.zero, FieldType.Input));
        AddField(new FloatField(this, "thing1", 0, FieldType.Input));
        AddField(new IntField(this, "thing2", 0, FieldType.Input));
        AddField(new StringField(this, "thing3", "0", FieldType.NoInput));
        AddField(new StringField(this, "thing4", "0", FieldType.Static, false));
        AddField(new NodeField(this));
        //AddField(new Vector3Field("frequency", Vector3.zero, FieldType.Input));
        AddField(new OutputField(this, Color.black));
    }
    
    public string GetFunctionString()
    {
        return "Error";
    }

    public object GetOutput()
    {
        throw new System.NotImplementedException();
    }
}
