sampler sample : register(s0);

float4 uShaderSpecificData;
float2 globalTime;
//float2 ScreenSize;

float matrixbase1;
float matrixbase2;
float4 ColorArray[4];
float multiplier;

float4 posterize(float4 color, float numColors)
{
    return floor(color * numColors + 0.5) / numColors;
}
float2x2 R(float a)
{
    return float2x2(cos(a + float4(0, 33, 11, 0)));
}
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 a = float2(0, 0);
    float2 L = float2(0, 0);
    float2 res = float2(0, 0);
    
    float s = 10.;
    for (float j = 0.; j < 17; j++)
    {
        coords = mul(coords, R(matrixbase1));
        a = mul(a, R(matrixbase2));
        L = coords * s + j + a - globalTime;
        a += sin(L * multiplier);
        res += (.5 + .5 * cos(L * multiplier)) / s;
        s *= 1.2 - .07 * .1;
    }
    float r = res.x + res.y;
    float4 Col = lerp(
        lerp(ColorArray[0], ColorArray[1], smoothstep(1., 1., clamp(r, 0, 1))),
        lerp(ColorArray[2], ColorArray[3], smoothstep(.5, 1., clamp(r, 0, 1))),
        smoothstep(0., 1., clamp(r, 0, 1))
    ) * tex2D(sample, coords);
    return posterize(Col, 16);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}