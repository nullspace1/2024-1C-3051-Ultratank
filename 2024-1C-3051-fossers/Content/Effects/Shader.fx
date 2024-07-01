#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_5_0
    #define PS_SHADERMODEL ps_5_0
#endif

float4x4 World;
float4x4 WorldLightViewProjection;
float4x4 WorldCameraViewProjection;
float4x4 InverseTransposeWorld;
float4x4 Bone;
float4x4 InverseTransposeBone;

float3 LightDirection;

float TextureSize;


float3 Color;
bool HasTexture;
float DiffuseCoefficient;

float FarPlaneDistance;
float NearPlaneDistance;
float3 ImpactPoints[5];
float3 ImpactVelocities[5];
int ImpactCount;

float3 LightPosition;
float3 LightColor;
bool Depth;

Texture2D Texture;
Texture2D ShadowDepth;
sampler2D shadowMapSampler = sampler_state
{
    Texture = (ShadowDepth);
     MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
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
    float4 WorldPosition : TEXCOORD3;
};


float GetRealDistance(float dist){
    return dist * FarPlaneDistance;
}


VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    output.UV = input.UV;
    output.WorldPosition = mul(mul(input.Position,Bone),World);
    output.Normal = mul(input.Normal,InverseTransposeWorld);

    float OFFSET = 10;

    float3 position = mul(input.Position,Bone).xyz;
    float distanceThreshold = 115;


    if (ImpactCount > 0) {
        float dis = distance(position, ImpactPoints[0]);
        if (dis < distanceThreshold) {
            position -= ImpactVelocities[0] * (1-dis/distanceThreshold) * 60;
        }
    }

    if (ImpactCount > 1) {
        float dis = distance(position, ImpactPoints[1]);
        if (dis < distanceThreshold) {
            position -= ImpactVelocities[1] * (1-dis/distanceThreshold) * 100;
        }
    }

    if (ImpactCount > 2) {
        float dis = distance(position, ImpactPoints[2]);
        if (dis < distanceThreshold) {
            position -= ImpactVelocities[2] * (1-dis/distanceThreshold) * 100;
        }
    }

    if (ImpactCount > 3) {
        float dis = distance(position, ImpactPoints[3]);
        if (dis < distanceThreshold) {
            position -= ImpactVelocities[3] * (1-dis/distanceThreshold) * 100;
        }
    }

    if (ImpactCount > 4) {
        float dis = distance(position, ImpactPoints[4]);
        if (dis < distanceThreshold) {
            position -= ImpactVelocities[4] * (1-dis/distanceThreshold) * 100;
        }
    }

    output.Position = mul(float4(position,1), WorldCameraViewProjection);
    output.LightSpacePosition = mul(float4(position,1), WorldLightViewProjection);

    return output;
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

float4 color = float4(base.rgb * NdotL * DiffuseCoefficient * shadow, base.a);


    return color;
}


float4 LightPS(in VertexShaderOutput input) : SV_TARGET
{
    if (Depth == true) return float4(0,0,0,1);

    float3 L = LightPosition - input.WorldPosition.xyz;
    float distance = length(L);
    L = normalize(L);
    float3 N = normalize(input.Normal.xyz);
    float NdotL = saturate(dot(L, N));

    float4 color = float4(saturate(LightColor * NdotL), 1);

    return color;
}


technique LightPass
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL LightPS();
    }
}

technique Default
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};