sampler baseTexture : register(s0);
sampler noiseTexture : register(s1);
sampler transmitTexture : register(s2);

float4 uShaderSpecificData;

float4 laserColor;
float4 baseColor;

float laserStartPercentage;

float centerIntensity;

float globalTime;

float stepSize;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
        // Intensity increases closer to the center
    float intensity = 1.0 - abs(coords.y - 0.5);
    
    intensity = pow(intensity, 5.4);
    
        // Decrease the intensity the at the start of the laser
    if (coords.x < laserStartPercentage)
    {
        intensity = lerp(0.0, intensity, pow(coords.x / laserStartPercentage, 0.7));
    }
    
        // Make look fast
    float2 samplePoint = coords;
    samplePoint.x = samplePoint.x * 0.1 - (globalTime * 3.2); // really stretched
    samplePoint.y = samplePoint.y * 3.0 + (globalTime * 0.25); // slow
    
        // Get Textures
    float noise = tex2D(noiseTexture, samplePoint).g;
    float4 center = tex2D(transmitTexture, -samplePoint);
    
        // Color fuckery
    float4 noiseColor = noise * laserColor;
    
    float4 effectColor = noiseColor * intensity * 19.0;
    
    effectColor = effectColor + center * centerIntensity * (pow(intensity, 4.0) * noise);
    
        // mix the laser color with the whole effect
    float4 laserCol = lerp(laserColor, effectColor, 0.78);
    
        // bright
    float4 baseCol = baseColor * pow(1.0 - intensity, 111.0);
    intensity *= 3.0;
    baseCol = lerp(baseCol, laserCol, intensity);
    
        // prolly stupid way to see if color is white,,
    if (length(baseCol) >= 1. * 5.9)
    {
        baseCol = float4(0., 0., 0., 1.);
    }
    else
    {
            // woke quantization
        baseCol = round(baseCol * stepSize) / stepSize;
    }
    
    return baseCol;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}