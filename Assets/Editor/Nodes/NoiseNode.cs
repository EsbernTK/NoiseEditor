using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class NoiseNode : BaseNode, IHasFunction, IHasOutput {
    public NoiseNode() : base()
    {
        windowName = "NoiseNode";
        AddField(new StringField(this, "Window Name", windowName, FieldType.NoInput));
        AddField(new Vector2IntField(this, "Position", new Vector2Int(0,0), FieldType.Input));
        AddField(new FloatField(this, "Amplitude", 1, FieldType.Input));
        AddField(new FloatField(this, "Frequency", 1, FieldType.Input));
        AddField(new IntField(this, "Octaves", 1, FieldType.Input));
        AddField(new FloatField(this, "Lacunarity", 2, FieldType.Input));
        AddField(new FloatField(this, "Persistence", 0.1f, FieldType.Input));
        AddField(new OutputField(this, Color.black));
        
    }
    public string GetFunctionString()
    {
        return null;
    }
    public override string GetFunctionCall(string givenNodeName, List<NodeTreeInfo> tree)
    {
        Dictionary<string, string> VarNameDict = new Dictionary<string, string>();
        foreach(AbstractField f in fields)
        {
            if(f.isShaderVariable == IsFieldShaderVariable.Yes)
            {
                VarNameDict.Add(f.name, givenNodeName + f.name.Replace(" ", ""));
            }
        }
        
        return "PerlinSum(" +VarNameDict["Position"] +","  + VarNameDict["Frequency"] +"," + VarNameDict["Octaves"] + "," + VarNameDict["Amplitude"] + "," + VarNameDict["Lacunarity"] + "," + VarNameDict["Persistence"] + ");";
    }
    public override string GetMainFunctionDeclarations()
    {
        //string previousFunctions = GetPreviousFunctions() + "\n";
        return
   @"   float3 PerlinSum(float2 Point, float frequency, int octaves, float amplitude, float lacunarity, float persistence)
        {

            float3 sum = PerlinNoise(Point, frequency);
            float range = 1;
            float tempfrequency = frequency;
            float tempOctaves = octaves;
            float2 derivativeSum = sum.xy; 

            uint val = min(tempOctaves, 8);
            for (uint o = 1; o < val; o++)
            {
                tempfrequency *= lacunarity;
                amplitude *= persistence;
                range = (range + amplitude); 

                float3 noise = PerlinNoise(Point, tempfrequency);
                float2 derivative = noise.yz;
                derivativeSum += derivative;
                sum = sum + (noise.x) * amplitude;

            }
            return float3(sum.x / range, derivativeSum / range);
        }
        ";
    }

    public object GetOutput()
    {
        throw new System.NotImplementedException();
    }






    public override string GetBaseFunctionDeclarations()
    {
        return 
@"float lerp(float a, float b, float w)
    {
        return a + w * (b - a);
    }
    
    float fract(float v)
    {
        return v - floor(v);
    }
    float hash(float n)
    {
        return fract(cos(n) * 41415.92653);
    }

    float Dot(float2 g, float x, float y)
    {
        return g.x * x + g.y * y;
    }
    float Smooth(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
    float SmoothDerivative(float t)
    {
        return 30 * t * t * (t * (t - 2) + 1);
    }

    float3 PerlinNoise(float2 Point, float frequency)
    {
        float2 gradients2D[] =
        {
        
            float2(1, 0),
		
        float2(-1, 0),
		
        float2(0, 1),
		
        float2(0, -1),
		
        float2(0.70710678118654752440084436210485, 0.70710678118654752440084436210485),
		
        float2(-0.70710678118654752440084436210485, 0.70710678118654752440084436210485),
		
        float2(0.70710678118654752440084436210485, -0.70710678118654752440084436210485),
		
        float2(-0.70710678118654752440084436210485, -0.70710678118654752440084436210485),

        };

        uint gradientMask = 8;

        Point *= frequency;
        float ix0 = floor(Point.x);

        float tx0 = fract(Point.x);
        float tx1 = tx0 - 1;

        float iy0 = floor(Point.y);
        float ty0 = fract(Point.y);
        float ty1 = ty0 - 1;

        float ix1 = ix0 + 1;
        float iy1 = iy0 + 1;

        float h0 = hash(ix0);
        float h1 = hash(ix1);

        //Retreiving Gradients
        float2 g00 = gradients2D[(int) (hash(h0 + iy0) * 423) % gradientMask];
        float2 g10 = gradients2D[(int) (hash(h1 + iy0) * 423) % gradientMask];

        float2 g01 = gradients2D[(int) (hash(h0 + iy1) * 423) % gradientMask];
        float2 g11 = gradients2D[(int) (hash(h1 + iy1) * 423) % gradientMask];
        //Calulating value at points
        float v00 = Dot(g00, tx0, ty0);
        float v10 = Dot(g10, tx1, ty0);
        float v01 = Dot(g01, tx0, ty1);
        float v11 = Dot(g11, tx1, ty1);

        float dtx = SmoothDerivative(tx0);
        float dty = SmoothDerivative(ty0);

        float tx = Smooth(tx0);
        float ty = Smooth(ty0);

        float a = v00;
        float b = v10 - v00;
        float c = v01 - v00;
        float d = v11 - v01 - v10 + v00;

        float2 da = g00;
        float2 db = g10 - g00;
        float2 dc = g01 - g00;
        float2 dd = g11 - g01 - g10 + g00;
        float2 derivative;
        derivative = da + db * tx + (dc + dd * tx) * ty;
        derivative.x += (b + d * ty) * dtx;
        derivative.y += (c + d * tx) * dty;


        float lerp0 = lerp(v00, v10, tx);
        float lerp1 = lerp(v01, v11, tx);
        float lerp3 = lerp(lerp0, lerp1, ty);

        return float3((lerp3), derivative.x, derivative.y) * 1.4142135623730950488016887242096980785696718753769480731766797379907324784621070;
    }";
    }
}
