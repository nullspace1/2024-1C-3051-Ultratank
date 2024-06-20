#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_5_0
    #define PS_SHADERMODEL ps_5_0
#endif

float4x4 WorldViewProjection;

float ImpactRadius;
float3 Impacts[5];
Texture2D Texture;
float3 Color;
bool HasTexture;

sampler2D textureSampler : register(s0)
{
    Texture = (Texture);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 UV : TEXCOORD0;
};

VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, WorldViewProjection);
    output.UV = input.UV;

    float3 position = input.Position.xyz;
    float dentDepth = 0.5;

    for (int i = 0; i < 5; ++i)
    {
        float3 impactPoint = Impacts[i];
        float dis = distance(position, impactPoint);

        if (dis < ImpactRadius)
        {
            float dentEffect = (1 - (dis / ImpactRadius)) * dentDepth;
            position -= input.Normal * dentEffect;
        }
    }

    output.Position = mul(float4(position, 1.0), WorldViewProjection);
    return output;
}

float4 MainPS(VertexShaderOutput input) : SV_Target
{
    return HasTexture ? tex2D(textureSampler, input.UV) : float4(Color.rgb, 1.0);
}

technique Default
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
