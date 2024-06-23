#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D Texture;

sampler2D textureSampler = sampler_state {
    Texture = (Texture);
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
    float4 color = tex2D(textureSampler, input.UV);
    return color;
}

technique Quad {
    pass P0 {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
