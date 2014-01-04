uniform extern texture ScreenTexture;

sampler diffuse_tex = sampler_state
{
    Texture = <ScreenTexture>;    
};


float g_fMiddleGrey = 0.6f;
float g_fMaxLuminance = 16.0f; 
static const float3 LUM_CONVERT = float3(0.299f, 0.587f, 0.114f);
float3 ToneMap(float3 vColor)
{
	// Средний цвет изображения в однопиксельном семплере.
	//float fLumAvg = tex2D(PointSampler1, float2(0.5f, 0.5f)).r;
	float fLumAvg = 0.1;

	// Освещённость текущего пиксела.
	float fLumPixel = dot(vColor, LUM_CONVERT);    

	// Оператор из "Photographic Tone Reproduction for Digital Images".
	float fLumScaled = (fLumPixel * g_fMiddleGrey) / fLumAvg;    
	float fLumCompressed = (fLumScaled * (1 + (fLumScaled / (g_fMaxLuminance * g_fMaxLuminance)))) / (1 + fLumScaled);
	return fLumCompressed * vColor;
}


float4 PixelShader(float2 texCoord: TEXCOORD0) : COLOR
{
    float4 color = tex2D(diffuse_tex, texCoord);
    
    // L = 0.27R + 0.67G + 0.06B    
	//float lum = 0.27 * color.r + 0.67 * color.g + 0.06 * color.b;
	//float lum = max( color.r, max( color.g, color.b));
	
    color = float4(ToneMap(color.rgb),1);
    color -= 0.04; color = max(color, 0.0f);
	static const float3 LUM_CONVERT = float3(0.299f, 0.587f, 0.114f);
	float lum = dot(color.rgb,LUM_CONVERT);
    
    if( lum < 0.147)
		color = float4(0,0,0,0);
	
	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
    }
}
