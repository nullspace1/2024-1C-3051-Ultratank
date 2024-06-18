

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
float4x4 DownLightViewProjection;
bool Depth;


texture DepthMap;

sampler2D shadowMapSampler =
sampler_state
{
	Texture = <DepthMap>;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

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
     float4 LightSpacePos : TEXCOORD2; // Add LightSpacePos to store position in light space
};

LightVertexShaderOutput LightVS(in LightVertexShaderInput input)
{
    LightVertexShaderOutput output = (LightVertexShaderOutput)0;
    output.Position = mul(input.Position, WorldViewProjection);
    output.Normal = mul(input.Normal, InverseTransposeWorld);
    output.WorldPosition = mul(input.Position, World);
output.LightSpacePos = mul(output.WorldPosition, DownLightViewProjection); // Transform position to light space
    return output;
}

float GetDepth(float depth){
    return (depth - 0.1) / (10000 - 0.1);
}

float4 LightPS(in LightVertexShaderOutput input) : SV_TARGET
{
    if (Depth == true) return float4(0,0,0,1);
    
    float3 L = normalize(LightPosition - input.WorldPosition.xyz);
    float3 N = normalize(input.Normal.xyz);
    float NdotL = dot(L, N);

    float dist = distance(LightPosition,input.WorldPosition.xyz) + 0.1;

    float shadowBias = 0.0000015;
    float angleBias = max(0.0000005 * (1.0 - NdotL), 0.0);

    float3 lightSpacePos1 = input.LightSpacePos.xyz / input.LightSpacePos.w;
    float2 shadowMapTextureCoordinates1 = 0.5 * lightSpacePos1.xy + float2(0.5, 0.5);
    shadowMapTextureCoordinates1.y = 1.0f - shadowMapTextureCoordinates1.y;


    float shadow = 0.0;
    float2 texelSize = float2(1.0 / 4096, 1.0 / 4096);

    for (int x = -1; x <= 1; ++x)
    {
        for (int y = -1; y <= 1; ++y)
        {
            float2 sampleCoords = shadowMapTextureCoordinates1 + float2(x, y) * texelSize;
            float depth = tex2D(shadowMapSampler, sampleCoords).r * 10000;
            shadow += (lightSpacePos1.z - shadowBias - angleBias > depth) ? 0.0 : 1.0;
        }
    }

    shadow /= 9.0; 


    float4 color = float4(saturate(LightColor * NdotL * shadow / dist * 5000), 1);
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