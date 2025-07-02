using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArithmaticNode : BaseNode, IHasFunction, IHasInput, IHasOutput
{
    
    public ArithmaticNode()
    {
        windowName = "ArithmeticNode";
        AddField(new StringField(this, "Window Name",windowName, FieldType.NoInput));
        AddField(new EnumField(this, "Operator", Operators.Addition, FieldType.NoInput));
        AddField(new FloatField(this, "Input 1", 0, FieldType.Input));
        AddField(new FloatField(this, "Input 2", 0, FieldType.Input));
        AddField(new OutputField(this, Color.black));
    }

    public string GetFunctionString()
    {
        throw new System.NotImplementedException();
    }

    public object GetOutput()
    {
        throw new System.NotImplementedException();
    }
}
public enum Operators { Addition,Subtraction,Division,Multiply }