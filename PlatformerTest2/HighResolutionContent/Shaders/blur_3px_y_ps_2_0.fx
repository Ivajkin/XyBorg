uniform extern texture ScreenTexture;

sampler diffuse_tex = sampler_state
{
    Texture = <ScreenTexture>;    
};

float4 PixelShader(float2 texCoord: TEXCOORD0) : COLOR
{
    //const float x_px_size = 1.0/1280.0; -> a=0.00078125		-> a*2 = 0.0015625		-> a*3 = 0.00234375		-> a*0.5 = 0.000390625		-> a*1.5 = 0.001171875
    //const float y_px_size = 1.0/720.0; ->  b=0.00138888(8)	-> b*2 = 0.0027777(7)	-> b*3 = 0.00416666(6)	-> b*0.5 = 0.000694444(4)	-> b*1.5 = 0.002083333(3)
    
    /*float4 color
		= tex2D(diffuse_tex, texCoord)
		+ tex2D(diffuse_tex, float2(texCoord.x, texCoord.y + 0.000694444))
		+ tex2D(diffuse_tex, float2(texCoord.x, texCoord.y + 0.00138888))
		+ tex2D(diffuse_tex, float2(texCoord.x, texCoord.y + 0.002083333))
		+ tex2D(diffuse_tex, float2(texCoord.x, texCoord.y - 0.000694444))
		+ tex2D(diffuse_tex, float2(texCoord.x, texCoord.y - 0.00138888))
		+ tex2D(diffuse_tex, float2(texCoord.x, texCoord.y - 0.002083333));*/
	
	float k = 19.751/3.0;
	float4 color
		= tex2D(diffuse_tex, texCoord)
		+ tex2D(diffuse_tex, float2(texCoord.x, texCoord.y + 1.0/720.0*k))
		+ tex2D(diffuse_tex, float2(texCoord.x, texCoord.y + 2.0/720.0*k))
		+ tex2D(diffuse_tex, float2(texCoord.x, texCoord.y + 3.0/720.0*k))
		+ tex2D(diffuse_tex, float2(texCoord.x, texCoord.y - 1.0/720.0*k))
		+ tex2D(diffuse_tex, float2(texCoord.x, texCoord.y - 2.0/720.0*k))
		+ tex2D(diffuse_tex, float2(texCoord.x, texCoord.y - 3.0/720.0*k));
    
    color /= 7;
    
	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
    }
}
