sampler mask : register(s0);
sampler img : register(s1);
float uAngle;
float4 uShaderSpecificData;

float map(float value, float start1, float stop1, float start2, float stop2)
{
    return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
}

    // fuck you heres the shader toy link you fucking pussy https://www.shadertoy.com/view/MfcfWX
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float dx = (coords.x - 0.5) * 2;
    float dy = (coords.y - 0.5) * 2;

    float scaleY = sqrt(1 - dy * dy);
    float angY = 0.5 + asin(dy) / 3.1416;
    
        // update: fixed stretching bug
    float angX = (0.5 + asin(dx / scaleY) / 3.1416) / 2 + uAngle;

    float4 col = tex2D(img, float2(angX, angY));
    col *= lerp(1, 0, map(clamp(dx * dx + dy * dy, 0.92, 1), 0.92, 1, 0, 1));
    return col * sampleColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}