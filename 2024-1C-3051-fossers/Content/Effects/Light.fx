#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_5_0
    #define PS_SHADERMODEL ps_5_0
#endif

float4x4 WorldViewProjection;
float4x4 InverseTransposeWorld;
float4x4 World;
float3 LightPosition;
float3 LightColor;
bool Depth;


struct LightVertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL;
};

struct LightVertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Normal : TEXCOORD0;
    float4 WorldPosition : TEXCOORD1;
    float4 LightSpacePos : TEXCOORD2;
};

LightVertexShaderOutput LightVS(in LightVertexShaderInput input)
{
    LightVertexShaderOutput output = (LightVertexShaderOutput)0;
    output.Position = mul(input.Position, WorldViewProjection);
    output.Normal = mul(input.Normal, InverseTransposeWorld);
    output.WorldPosition = mul(input.Position, World);
    return output;
}

float4 LightPS(in LightVertexShaderOutput input) : SV_TARGET
{
    if (Depth == true) return float4(0,0,0,1);

    float3 L = LightPosition - input.WorldPosition.xyz;
    float distance = length(L);
    L = normalize(L);
    float3 N = normalize(input.Normal.xyz);
    float NdotL = saturate(dot(L, N));

    // Calculate attenuation
    float attenuation = 1;

    float4 color = float4(saturate(LightColor * NdotL * attenuation), 1);

    return color;
}

technique BlackPass
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL LightVS();
        PixelShader = compile PS_SHADERMODEL LightPS();
    }
};