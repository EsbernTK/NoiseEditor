using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Linq;
[System.Serializable]
public class ShaderCreator {
    ComputeShader shader;
    public string shaderString;

    public void CreateShader(List<NodeTreeInfo> tree)
    {
        //shader = ShaderUtil.CreateShaderAsset(baseShader);
        shaderString = GetBaseShader();
        string variableBlock = CreateShaderVariableBody(tree);
        string baseFunctionBlock = CreateShaderBaseFunctionBody(tree);
        string mainFunctionBlock = CreateShaderMainFunctionBody(tree);
        string functionCallBlock = CreateShaderFunctionCallBody(tree);
        shaderString = string.Format(shaderString, baseFunctionBlock + "\n" + mainFunctionBlock, variableBlock, functionCallBlock).Replace("¤", "{").Replace("$", "}");
        using (FileStream fs = File.Create("Assets/Noise Assets/Resources/test1.compute"))
        {
            AddText(fs, shaderString);
        }
        AssetDatabase.ImportAsset("Assets/Noise Assets/Resources/test1.compute", ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();
        //Now compile the shader
        Debug.Log("Shader created with content:\n" + shaderString);
        //ShaderUtil.CreateShaderAsset(shaderString);
        RunShader();
    }
    public void RunShader()
    {
        ComputeShader test = (ComputeShader)Resources.Load("test1");

        ComputeBuffer test_buff = new ComputeBuffer(128 * 128, sizeof(float) * 3, ComputeBufferType.Default);
        Vector3[] cols = new Vector3[128 * 128];
        int index = test.FindKernel("CSMain");
        test.SetBuffer(index, "Result", test_buff);
        test.Dispatch(index, 128 / 8, 128 / 8, 1);
        test_buff.GetData(cols);
        //test_buff.Dispose();
        List<Vector3> results = cols.ToList();//.OrderByDescending(x => x.x).ToList();
        Debug.Log(results[0]);
        test_buff.Dispose();

    }



    string CreateShaderFunctionCallBody(List<NodeTreeInfo> tree)
    {
        string functionCallBlock = "";
        foreach (NodeTreeInfo n in tree)
        {
            string tempVariableBlock = "float3 " + n.varName + " = "+ n.node.GetFunctionCall(n.varName,tree);
            functionCallBlock += tempVariableBlock + "\n";
        }
        functionCallBlock += "noiseResult = " + tree[tree.Count - 1].varName + ".x;";
        return functionCallBlock;
    }

    string CreateShaderVariableBody(List<NodeTreeInfo> tree)
    {
        string variableBlock = "";
        foreach (NodeTreeInfo n in tree)
        {
            string tempVariableBlock = n.node.GetVariableDeclarations(n.varName);
            variableBlock += tempVariableBlock + "\n";
        }

        return variableBlock;
    }

    string CreateShaderBaseFunctionBody(List<NodeTreeInfo> tree)
    {
        string functionBlock = "";
        Dictionary<string, string> seenTypes = new Dictionary<string, string>();
        foreach (NodeTreeInfo n in tree)
        {
            string typeName = n.node.GetType().ToString();
            if (seenTypes.ContainsKey(typeName))
            {
                continue;
            }
            seenTypes.Add(typeName,typeName);
            string tempVariableBlock = n.node.GetBaseFunctionDeclarations();
            functionBlock += tempVariableBlock + "\n";
        }

        return functionBlock;
    }
    string CreateShaderMainFunctionBody(List<NodeTreeInfo> tree)
    {
        string functionBlock = "";
        Dictionary<string, string> seenTypes = new Dictionary<string, string>();
        foreach (NodeTreeInfo n in tree)
        {
            string typeName = n.node.GetType().ToString();
            Debug.Log("TypeName: " + typeName);
            if (seenTypes.ContainsKey(typeName))
            {
                continue;
            }
            string tempVariableBlock = n.node.GetMainFunctionDeclarations();
            functionBlock += tempVariableBlock + "\n";
        }

        return functionBlock;
    }

    string GetBaseShader()
    {
        return @"   #pragma kernel CSMain 
    {0}
    {1}
    RWTexture2D<float> Result;
    float noiseResult;
    int xWidth = 128;


    [numthreads(8, 8, 1)]
    void CSMain(uint2 id : SV_DispatchThreadID)
    ¤
        noiseResult = 0;
        {2}
        Result[id.xy] = noiseResult;
    $";
    //Result[int((id.x) + xWidth * id.y)] = noiseResult;
    }

    private static void AddText(FileStream fs, string value)
    {
        byte[] info = new UTF8Encoding(true).GetBytes(value);
        fs.Write(info, 0, info.Length);
    }
}
