sampler eye : register(s0);

float4 uShaderSpecificData;
float globalTime;
float GlitchAmount;
float2 ScreenSize;

float4 posterize(float4 src, float steps)
{
    return floor(src * steps + 0.5) / steps;
}
float2 quantize(float2 v, float steps)
{
    return floor(v * steps) / steps;
}
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    coords /= ScreenSize;
    float amount = pow(GlitchAmount, 2.0);
    float2 pixel = 3.0;
    float4 color = tex2D(eye, coords);
    float t = (globalTime + amount * (amount - 0.2)) * 2100;
    float4 postColor = posterize(color, 4.0);
    float4 a = posterize(tex2D(eye, quantize(coords, 64.0 * t) + pixel * (postColor.rb - float2(.5, .5)) * 100.0), 5.0).rbga;
    float4 b = posterize(tex2D(eye, quantize(coords, 32.0 - t) + pixel * (postColor.rg - float2(.5, .5)) * 1000.0), 4.0).gbra;
    float4 c = posterize(tex2D(eye, quantize(coords, 16.0 + t) + pixel * (postColor.rg - float2(.5, .5)) * 20.0), 16.0).bgra;
    return lerp(
        tex2D(eye, coords + amount * (
            quantize((a * t - b + c - (t + t / 2.0) / 10.0).rg, 16.0) - float2(.5, .5)
        ) * pixel * 100.0),
        (a + b + c) / 3.0,
        (0.5 - (dot(color, postColor) - 1.5)) * amount
    );
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}