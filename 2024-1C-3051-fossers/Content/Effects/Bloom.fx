#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D Screen;
float2 LightScreenPosition;
float2 ScreenSize;
float3 LightColor;
float Distance;

sampler2D textureSampler = sampler_state {
    Texture = (Screen);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VS_INPUT {
    float4 Pos : SV_POSITION;
    float2 UV : TEXCOORD0;
};

struct VS_OUTPUT {
    float4 Pos : SV_POSITION;
    float2 UV : TEXCOORD0;
};

VS_OUTPUT MainVS(VS_INPUT input) {
    VS_OUTPUT output = (VS_OUTPUT)0;
    output.Pos = input.Pos;
    output.UV = input.UV;
    return output;
}

float4 MainPS(VS_OUTPUT input) : SV_TARGET {

    float distanceToLight = distance(LightScreenPosition, input.UV);
    float intensity = smoothstep(0,0.06,distanceToLight);
    
    float3 color = float3(0,0,0);

     color = LightColor * (1-intensity);
    
     
    return float4(tex2D(textureSampler,input.UV).rgb + color / Distance,1) ;

}



technique Quad {
    pass P0 {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
