#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_5_0
    #define PS_SHADERMODEL ps_5_0
#endif


float4x4 WorldViewProjection;

struct DepthPassVertexShaderInput
{
	float4 Position : POSITION0;
};

struct DepthPassVertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 ScreenSpacePosition : TEXCOORD1;
};

DepthPassVertexShaderOutput DepthVS(in DepthPassVertexShaderInput input)
{
	DepthPassVertexShaderOutput output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.ScreenSpacePosition = mul(input.Position, WorldViewProjection);
	return output;
}

float GetDepth(float depth){
    return (depth) / (10000);
}


float4 DepthPS(in DepthPassVertexShaderOutput input) : SV_TARGET
{
    float depth = GetDepth(input.ScreenSpacePosition.z / input.ScreenSpacePosition.w);
    return float4(depth,depth,depth, 1.0);
}

technique DepthPass
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL DepthVS();
		PixelShader = compile PS_SHADERMODEL DepthPS();
	}
};