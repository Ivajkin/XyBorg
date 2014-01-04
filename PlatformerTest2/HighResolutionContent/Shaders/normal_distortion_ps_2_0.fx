uniform extern texture ScreenTexture;
uniform extern texture DistortionTexture;

sampler ScreenS = sampler_state
{
    Texture = <ScreenTexture>;    
};

sampler distortionTexture = sampler_state
{
    Texture = <DistortionTexture>;
    mipfilter = LINEAR; 
};

#define INTERFERENCE_ENABLED

float4 PixelShader(float2 texCoord: TEXCOORD0) : COLOR
{
    float4 normal = tex2D(distortionTexture, texCoord);
    
#ifdef INTERFERENCE_ENABLED
    
    float4 color = float4(0,0,0,1);
    float k = (normal.b-0.5)/12;
    float x = (normal.r-0.5)*3.1415;
    float y = (normal.g-0.5)*3.1415;
    float2 sin3 = float2(x,y);
    float2 sin2 = float2(x*1.1,y*1.1);
    float2 sin1 = float2(x*1.3,y*1.3);
    
    color.r = tex2D(ScreenS, float2(texCoord.x+k*sin1.x,texCoord.y+k*sin1.y)).r;
    color.g = tex2D(ScreenS, float2(texCoord.x+k*sin2.x,texCoord.y+k*sin2.y)).g;
    color.b = tex2D(ScreenS, float2(texCoord.x+k*sin3.x,texCoord.y+k*sin3.y)).b;
#else
    float k1 = sin((normal.r-0.5)*3.1415)*(normal.b-0.5)/12;
    float k2 = sin((normal.g-0.5)*3.1415)*(normal.b-0.5)/12;
    float4 color = tex2D(ScreenS, float2(texCoord.x+k1,texCoord.y+k2));
#endif

	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
    }
}
