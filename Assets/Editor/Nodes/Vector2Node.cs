using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Vector2Node : BaseNode, IHasOutput
{

	public Vector2Node()
    {
        AddField(new Vector2Field(this,"", Vector2.zero, FieldType.Output));
    }

    public object GetOutput()
    {
        throw new System.NotImplementedException();
    }
}
