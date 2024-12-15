sampler baseTexture : register(s0);
sampler ditherTexture : register(s1);

float4 uShaderSpecificData;

float uTime;

float dithering;

float2 ScreenSize;

float3 scanlines(float2 uv, float scanSpeed)
{
    float scanlines = abs(cos((uv.y + scanSpeed) * 160));
    
    float scanlines_lower = abs(cos((uv.y + 0.01 + scanSpeed) * 160));
    float scanlines_upper = abs(cos((uv.y - 0.01 + scanSpeed) * 160));
    
    float3 white = float3(1, 1, 1);
    float3 gb = float3(0, 1, 1);
    float3 r = float3(1, 0, 0);
    
    return (white * scanlines) + (gb * scanlines_lower) + (r * scanlines_upper);
}
    
    // Taking from the original shader but i made it use way less operations i think :3
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = round(coords * ScreenSize) / ScreenSize;
    
    float scanSpeed = frac(uTime);
    
        // As stated in the original shader by @devrique this fisheye distortion was made by @ddoodm, you can find their original shader here: https://www.shadertoy.com/view/ltSXRz
    float2 eyefishuv = (coords - 0.5) * 3;
    float deform = (1 - eyefishuv.y * eyefishuv.y) * 0.02 * eyefishuv.x;
    
    float2 deformedCoords = float2(coords.x - deform * 0.95, coords.y);
    
    float3 col = scanlines(deformedCoords, scanSpeed) * 0.4;
        // Simple* Vignette
    float bottomRight = pow(uv.x, uv.y * 70);
    float bottomLeft = pow(1 - uv.x, uv.y * 70);
    float topRight = pow(uv.x, (1 - uv.y) * 70);
    float topLeft = pow(uv.y, uv.x * 70);
    
    float screenForm = 1 - (bottomRight + bottomLeft + topRight + topLeft);
    
        // because single float constuctors are a premium feature or some bs
    float3 vignette = float3(screenForm, screenForm, screenForm);
    
        // Get the deformed texture and add dithering
    float3 color = tex2D(baseTexture, float2(coords.x - deform * 0.95, coords.y)).rgb / dithering;
    float dither = tex2D(ditherTexture, deformedCoords).r;

    color += float3(dither, dither, dither) / 255.0;
        // And add the scanelines
    color = ((floor(color * 255.0) / 255.0) * dithering) + (col * 0.13);
    
        // Add vignette
    return float4(color * vignette, 1);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}