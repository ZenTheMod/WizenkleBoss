sampler Form : register(s0);
sampler Void : register(s1);

float4 uShaderSpecificData;

float4 baseColor;
float globalTime;

float min;
float max;

float2 screenPosition;

float2 MatrixTransform2x2(float2 uv)
{
    return mul(float2x2(0.841, 0.54, -0.841, 0.841), uv);
}

float map(float value, float start1, float stop1, float start2, float stop2)
{
    return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
}

float Coronaries(float2 uv, float time)
{
    float2 a = float2(0, 0);
    float2 res = float2(0, 0);
    float s = 12;
    
    for (float j = 0; j < 12; j++)
    {
        uv = MatrixTransform2x2(uv);
        a = MatrixTransform2x2(a);
        
        float2 L = uv * s + j + a - time;
        a += cos(L);
        res += (.5 * sin(L)) / s;
        s *= 1.2;
    }
    
    return res.x + res.y;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float blur = map(tex2D(Form, coords).b, 0, 1, min, max);
    float veins = Coronaries(coords * 3.4 + screenPosition, globalTime);
    
    float stars = tex2D(Void, coords + (screenPosition * 2)).b * blur * 8;
    
    float mixed = clamp(lerp(blur * 0.7, 1.4 - veins, 0.49) - 0.5, 0, 1) * 6;

    float valuebecausefuckyoufloatconstructor = stars + mixed;
    
    float4 col = float4(valuebecausefuckyoufloatconstructor, valuebecausefuckyoufloatconstructor, valuebecausefuckyoufloatconstructor, valuebecausefuckyoufloatconstructor) * baseColor * mixed * 1.2;
    return abs(col);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}