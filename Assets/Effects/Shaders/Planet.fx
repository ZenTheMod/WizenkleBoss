sampler planet : register(s0);
sampler shadow : register(s1);

float4 shadowColor;

float textureAngle;
float shadowAngle;
float falloffStart;

float4 uShaderSpecificData;

float map(float value, float start1, float stop1, float start2, float stop2)
{
    float clamped = clamp(value, start1, stop1);
    return start2 + (stop2 - start2) * ((clamped - start1) / (stop1 - start1));
}

    // https://www.shadertoy.com/view/MfcfWX
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float dx = (coords.x - 0.5) * 2;
    float dy = (coords.y - 0.5) * 2;

    float scaleY = sqrt(1 - dy * dy);
    float angY = 0.5 + asin(dy) / 3.1416;
    
    float angX = (0.5 + asin(dx / scaleY) / 3.1416) / 2;
    
    float textureAngX = angX + textureAngle;
    float shadowAngX = angX + shadowAngle;
    
    float4 col = tex2D(planet, float2(textureAngX, angY));
    float shade = tex2D(shadow, float2(shadowAngX, angY)).r;
    
    col = lerp(col * sampleColor, shadowColor, shade);
    
    col *= map(dx * dx + dy * dy, falloffStart, 1, 1, 0); // falloff
    return col;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}