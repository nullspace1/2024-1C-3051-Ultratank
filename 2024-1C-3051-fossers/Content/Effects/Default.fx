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
float4x4 WorldLightViewProjection;
float3 LightDirection;
Texture2D ShadowDepth;
float TextureSize;

Texture2D Texture;
float3 Color;
bool HasTexture;
float DiffuseCoefficient;

float FarPlaneDistance;
float NearPlaneDistance;

sampler2D shadowMapSampler = sampler_state
{
    Texture = (ShadowDepth);
     MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};


sampler2D textureSampler = sampler_state
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
    float4 Normal : NORMAL;
    float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 UV : TEXCOORD0;
    float4 Normal : TEXCOORD1;
    float4 LightSpacePosition : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    output.Position = mul(input.Position, WorldViewProjection);
    output.UV = input.UV;
    output.Normal = mul(input.Normal,InverseTransposeWorld);
    output.LightSpacePosition = mul(input.Position, WorldLightViewProjection);
    return output;
}

float GetRealDistance(float dist){
    return dist * FarPlaneDistance;
}


float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 base = HasTexture ? tex2D(textureSampler, input.UV) : float4(Color.rgb, 1);

    float shadowBias = 0.00001f;
    float angleBias = shadowBias / 2;
    
    float3 N = normalize(input.Normal.xyz);
    float3 L = normalize(LightDirection.xyz);

    float NdotL = saturate(dot(N,L));

    float3 lightSpacePos = input.LightSpacePosition.xyz / input.LightSpacePosition.w;
    float2 shadowMapTextureCoordinates1 = 0.5 * lightSpacePos.xy + float2(0.5, 0.5);
    shadowMapTextureCoordinates1.y = 1.0f - shadowMapTextureCoordinates1.y;

    float2 offsets[9] = {
        float2(-1.0, -1.0), float2(0.0, -1.0), float2(1.0, -1.0),
        float2(-1.0,  0.0), float2(0.0,  0.0), float2(1.0,  0.0),
        float2(-1.0,  1.0), float2(0.0,  1.0), float2(1.0,  1.0)
    };

    float weights[9] = {
        0.075, 0.125, 0.075,
        0.125, 0.200, 0.125,
        0.075, 0.125, 0.075
    };

    float shadow = 0.0;
    for (int i = 0; i < 9; ++i)
    {
        float2 sampleCoords = shadowMapTextureCoordinates1 + offsets[i]  * 1/TextureSize;
        float depth = tex2D(shadowMapSampler, sampleCoords).r;
        depth = GetRealDistance(depth);
        shadow += (lightSpacePos.z  - shadowBias - angleBias > depth) ? 0.5 * weights[i] : weights[i];
    }

    float4 color = float4(base.rgb * DiffuseCoefficient * NdotL * shadow, base.a);
    return color;
}

technique Default
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};