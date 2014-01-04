uniform extern texture ScreenTexture;
float f;
uniform extern texture VelocityTexture;

sampler ScreenS = sampler_state
{
    Texture = <ScreenTexture>;    
};
sampler velocityTexture = sampler_state
{
    Texture = <VelocityTexture>;
    mipfilter = LINEAR; 
};

float4 PixelShader(float2 texCoord: TEXCOORD0) : COLOR
{
    float4 velocity = tex2D(velocityTexture, texCoord);
    velocity.rgb *= 2;
    velocity.rgb -= float3(1,1,1);
    
    //float4 color = tex2D(ScreenS, texCoord);

	return velocity;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
    }
}
