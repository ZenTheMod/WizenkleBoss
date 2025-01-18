sampler text : register(s0);

float4 uShaderSpecificData;
float2 ScreenSize;
float Decay;
float RippleStrength;
float4 InkColor;

float S(float2 offset, float2 uv)
{
    return tex2D(text, uv + offset).r;
}

    // Credits to @4rknova https://www.shadertoy.com/view/3sB3WW

float4 Water(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(text, coords);
    
    float3 e = float3(1. / ScreenSize, 0.);
    
        // Sample the water cardinally, (the lazy way)
    float Center = c.g;
    float Up = S(-e.zy, coords);
    float Left = S(-e.xz, coords);
    float Right = S(e.xz, coords);
    float Down = S(e.zy, coords);
    
    float d = 0.;
    
        // Vodoo
    d = RippleStrength * smoothstep(0.0, 5.4, c.b);
    d += -(Center - .5) * 2. + (Up + Left + Right + Down - 2.);
    d *= Decay;
    
    d = d * 0.5 + 0.5;
    return float4(d, c.r, 0., 0);
}

float4 Ink(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 tc = tex2D(text, coords).rg;
    
    float n = length(clamp(tc - 0.54, 0., 1.));
    
    return float4(n, n, n, n) * InkColor * RippleStrength;
}

technique Technique1
{
    pass Processer
    {
        PixelShader = compile ps_2_0 Water();
    }
    pass InkColorizer
    {
        PixelShader = compile ps_2_0 Ink();
    }
}