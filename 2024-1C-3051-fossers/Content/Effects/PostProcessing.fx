#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D Screen;
float PixelationAmount = 6; // A new parameter to control the pixelation size

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
    float2 pixelSize = 500;
    float2 pixelatedUV = float2(floor(input.UV.x * pixelSize.x) / pixelSize.x,floor(input.UV.y * pixelSize.y) / pixelSize.y);

    // Average color over the 4 nearest texels
    float4 color = tex2D(textureSampler, pixelatedUV);


    return color;
}

technique Quad {
    pass P0 {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
