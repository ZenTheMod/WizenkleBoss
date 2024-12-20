sampler baseTexture : register(s0);
sampler maskTexture : register(s1);
sampler maskedTexture : register(s2);

float4 uShaderSpecificData;

float uTime;

float DrugStrength;

float4 embossColor;
float4 outlineColor;

float2 ScreenSize;

float MaskThreshold;

float contrast;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
        // Col is the normal screen in this case
    float4 col = tex2D(baseTexture, coords);
    
        // Contrast it so that the emboss looks better on it
    float3 contrastCol = ((col - 0.75) * max(3, 0)) + 1;
    
        // Mix the mask layer with some noise
    float mask = (tex2D(maskTexture, coords).r - MaskThreshold) * 20;
    mask = clamp(mask, 0, 1) * DrugStrength; // Clamp values cus x20
    
        // If this pixel is an outline then just skip everything
    if (distance(tex2D(maskedTexture, coords) * mask, outlineColor) < 0.05)
        return outlineColor;
    
        // Correct aspect ratio
    float2 aspect = 1.0 / ScreenSize;
    
    float2 dirAvg = float2(0, 0);
    
    for (float i = 0.0; i < 6.28318; i += 6.28318 / 16.)
    {
            // Find color difference in all directions
        float2 dir = float2(sin(i), cos(i));
        
            // Offset Coords
        float2 altCoords = coords + dir * aspect * 2;
        
            // Sample textures like a motherfucker
        float3 emboss = tex2D(baseTexture, altCoords);
        float3 maskedEmboss = tex2D(maskedTexture, altCoords); // This is for the masked texture, in this case the 'boss'
        
            // Add the samples to the average
        dirAvg += dir * distance(contrastCol, emboss) / 2; // Darken this one so the masked layer is more promenent
        dirAvg += dir * distance(contrastCol, maskedEmboss);
    }
    
        // Brighten (to make eyes hurt)
    dirAvg *= 3.0;
    
        // Make the emboss "spin"
    dirAvg.x = sin(uTime) * dirAvg.x;
    dirAvg.y = cos(uTime) * dirAvg.y;
    
        // Add the emboss layer to the base screen
    return float4((col.rgb - (mask * contrast)) + (((dirAvg.x * embossColor.rgb) + (dirAvg.y * embossColor.rgb)) * mask), 1);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}