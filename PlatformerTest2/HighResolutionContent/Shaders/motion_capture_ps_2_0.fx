uniform extern texture ScreenTexture;

sampler ScreenS = sampler_state
{
    Texture = <ScreenTexture>;    
};

float3 current_velocity;

float4 PixelShader(float2 texCoord: TEXCOORD0) : COLOR
{
    float4 color = float4( current_velocity, tex2D(ScreenS, texCoord).a);

	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
    }
}
