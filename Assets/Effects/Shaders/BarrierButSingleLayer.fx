sampler baseTexture : register(s0);

float4 uShaderSpecificData;

float uTime;

float4 embossColor;

float2 Size;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
        // Col is the normal screen in this case
    float4 col = tex2D(baseTexture, coords);
    
        // Correct aspect ratio
    float2 aspect = 1.0 / Size;
    
    float2 dirAvg = float2(0, 0);
    
    for (float i = 0.0; i < 6.28318; i += 6.28318 / 8.)
    {
            // Find color difference in all directions
        float2 dir = float2(sin(i), cos(i));
        
            // Offset Coords
        float2 altCoords = coords + dir * aspect * 3;
        
            // Sample textures like a motherfucker
        float4 emboss = tex2D(baseTexture, altCoords);
        
            // Add the samples to the average
        dirAvg += dir * distance(col, emboss);
    }
    
        // Brighten (to make eyes hurt)
    dirAvg *= 4.0;
    
        // Make the emboss "spin"
    dirAvg.x = sin(uTime) * dirAvg.x;
    dirAvg.y = cos(uTime) * dirAvg.y;
    
        // Add the emboss layer to the base color
    return (col + ((dirAvg.x * embossColor) + (dirAvg.y * embossColor))) * sampleColor;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}