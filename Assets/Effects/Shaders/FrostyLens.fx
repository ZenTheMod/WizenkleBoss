sampler lensTexture : register(s0);
sampler lensfrostTexture : register(s1);

float4 uShaderSpecificData;

float rand(float2 uv)
{
    return frac(sin(uv.x * 100. + uv.y * 6574.));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
        // the inverse of the planet shader.
    float2 d = (coords.xy - float2(0.5, 0.5)) * 2.;
    float2 scale = sqrt(float2(1, 1) + (d.xy * d.xy) / 6.);
    
    coords.x = 0.5 + asin(d.x / scale.y) / 3.1416;
    coords.y = 0.5 + asin(d.y / scale.x) / 3.1416;
    
        // generate random frost.
    float4 frost = tex2D(lensfrostTexture, coords);
    float2 rnd = float2(rand(coords + frost.r), rand(coords + frost.b));
    
    float dist = distance(coords, float2(0.5, 0.5));
    float vigfin = 1. - smoothstep(1.65, 0.21, dist);
   
    rnd *= .025 * vigfin + frost.rg * vigfin;
    coords += rnd / 4;
    float4 col = lerp(tex2D(lensTexture, coords), float4(1, 1, 1, 1), 6 * rnd.x);
    
    return col;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}