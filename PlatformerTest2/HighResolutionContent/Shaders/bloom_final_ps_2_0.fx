uniform extern texture ScreenTexture;
uniform extern texture GlowTex;

sampler ScreenS = sampler_state
{
    Texture = <ScreenTexture>;    
};

sampler glowTex = sampler_state
{
    Texture = <GlowTex>;
    mipfilter = LINEAR; 
};


float g_fMiddleGrey = 0.6f;
float g_fMaxLuminance = 16.0f; 
static const float3 LUM_CONVERT = float3(0.299f, 0.587f, 0.114f);
float3 ToneMap(float3 vColor)
{
	// Get the calculated average luminance
	//float fLumAvg = tex2D(PointSampler1, float2(0.5f, 0.5f)).r;
	float fLumAvg = 0.1;

	// Calculate the luminance of the current pixel
	float fLumPixel = dot(vColor, LUM_CONVERT);    

	// Apply the modified operator (Eq. 4)
	float fLumScaled = (fLumPixel * g_fMiddleGrey) / fLumAvg;    
	float fLumCompressed = (fLumScaled * (1 + (fLumScaled / (g_fMaxLuminance * g_fMaxLuminance)))) / (1 + fLumScaled);
	return fLumCompressed * vColor;
}


float4 PixelShader(float2 texCoord: TEXCOORD0) : COLOR
{
    float4 glow = tex2D(glowTex, texCoord);
    float4 color = tex2D(ScreenS, texCoord) + glow;
	
	/*// Reinhard's tone mapping equation simplified
	float Lp = max( color.r, max( color.g, color.b ) );
	float LmSqr = 3;
    float toneScalar = ( Lp * ( 1.0f + ( Lp / ( LmSqr ) ) ) ) / ( 1.0f + Lp );
    color = color * toneScalar * toneScalar;*/
    color = float4(ToneMap(color.rgb),1);

	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
    }
}
