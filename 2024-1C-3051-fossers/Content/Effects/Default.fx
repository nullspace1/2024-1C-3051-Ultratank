#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_5_0
    #define PS_SHADERMODEL ps_5_0
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 InverseTransposeWorld;
float4x4 LightViewProjection;


float3 LightSourcePositions[15]; 
float3 LightSourceColors[15];

float3 AmbientLight;
float3 AmbientLightDirection;
float AmbientCoefficient;
float DiffuseCoefficient;

Texture2D Texture;
Texture2D ShadowMap;
float3 Color;
bool HasTexture;

static const float modulatedEpsilon = 0.000041200182749889791011810302734375;
static const float maxEpsilon = 0.000023200045689009130001068115234375;

sampler2D textureSampler = sampler_state
{
    Texture = (Texture);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};



sampler2D shadowMapSampler =
sampler_state
{
	Texture = <ShadowMap>;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
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
    float4 WorldPos : TEXCOORD2;
    float4 LightSpacePos : TEXCOORD3; // Add LightSpacePos to store position in light space
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);    
    output.WorldPos = worldPosition;
    output.Position = mul(viewPosition, Projection);
    output.UV = input.UV;
    output.Normal = mul(input.Normal, InverseTransposeWorld);
    output.LightSpacePos = mul(worldPosition, LightViewProjection); // Transform position to light space
    return output;
}

float4 MainPS(VertexShaderOutput input) : SV_Target
{
    float3 DiffuseColor = float3(0, 0, 0);

    // Calculate shadow factor
    float shadowFactor = 1.0;
    float3 lightSpacePos = input.LightSpacePos.xyz / input.LightSpacePos.w;
    float2 shadowMapCoordinates = lightSpacePos.xy * 0.5 + float2(0.5,0.5);
    shadowMapCoordinates.y = 1.0f - shadowMapCoordinates.y;

    // Sample the shadow map
    float shadowMapDepth = tex2D(shadowMapSampler, shadowMapCoordinates).r;
    if (lightSpacePos.z  * 1 / 5000 > shadowMapDepth) // Add a small bias to avoid shadow acne
    {
        shadowFactor = 0.5; // In shadow
    }

    
    for (int i = 0; i < 15; i++)
    {
        float3 lightPosition = LightSourcePositions[i].xyz;
        float3 lightColor = LightSourceColors[i].rgb;

        float3 L = normalize(lightPosition - input.WorldPos.xyz);
        float3 N = normalize(input.Normal.xyz);

        float NdotL = dot(L, N);

        float distanceSq = length(lightPosition - input.WorldPos.xyz) * 0.001;
        float diffuseIntensity = saturate(NdotL);
        DiffuseColor += diffuseIntensity * lightColor * DiffuseCoefficient / distanceSq; 
    }

    float ambientIntensity = saturate(dot(normalize(AmbientLightDirection), normalize(input.Normal.xyz)));
    float3 ambientColor = AmbientLight * AmbientCoefficient * ambientIntensity;

    float levels = 10.0;
    float ambientBoost = 0.2f;
    ambientColor = floor(ambientColor * levels) / levels;
    DiffuseColor = floor((DiffuseColor + ambientColor * ambientBoost) * levels) / levels;

    float4 texelColor = HasTexture ? tex2D(textureSampler, input.UV) : float4(Color.rgb, 1);
    float3 finalColor = saturate(ambientColor + DiffuseColor) * texelColor.rgb;

    return float4(finalColor *  shadowFactor, texelColor.a);
}



/* --------------------------------------------- */



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

float4 DepthPS(in DepthPassVertexShaderOutput input) : SV_TARGET
{
    float depth = input.ScreenSpacePosition.z / input.ScreenSpacePosition.w * 1 / 5000;
    return float4(depth, depth, depth, 1.0);
}

technique DepthPass
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL DepthVS();
		PixelShader = compile PS_SHADERMODEL DepthPS();
	}
};

technique ActualDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
