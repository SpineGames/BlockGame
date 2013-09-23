#define MAX_LIGHTS 100

float4x4 World;
float4x4 View;
float4x4 Projection;
//float4x4 WorldInverseTranspose;
 
float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;
 
float3 DiffuseLightDirection = float3(1, 0, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;
 
int lightCount = 0;
 
texture Texture;
sampler2D textureSampler = sampler_state {
    Texture = (Texture);
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

float3 LightPositions[MAX_LIGHTS];
float4 diffuseColors[MAX_LIGHTS];
float  diffusePowers[MAX_LIGHTS];
float4 specularColors[MAX_LIGHTS];
float  specularPowers[MAX_LIGHTS];


struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};

struct ColoredVertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL0;
	float4 Color : COLOR0;
    float2 TextureCoordinate : TEXCOORD0;
};
 
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR1;
    float3 Normal : TEXCOORD0;
    float2 TextureCoordinate : TEXCOORD1;
};
 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
 
    float4 normal = normalize(mul(input.Normal, World)); //World-WorldInverseTranspose
    float lightIntensity = dot(normal, DiffuseLightDirection);
    output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity + AmbientColor * AmbientIntensity);
	output.Color.a = 1;
 
    output.Normal = normal;
 
    output.TextureCoordinate = input.TextureCoordinate;
    return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    textureColor.a = 1;

    return saturate(textureColor * input.Color);
}
 
technique Textured
{
    pass Pass1
    {
        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}