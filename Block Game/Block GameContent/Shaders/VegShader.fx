#define MAX_LIGHTS 100
#define TORAD 3.14159265359 / 180.0

float4x4 World;
float4x4 View;
float4x4 Projection;
//float4x4 WorldInverseTranspose;
 
float4 AmbientColor = float4(0, 0, 0, 1);
float AmbientIntensity = 0.1;
 
float3 DiffuseLightDirection = float3(1, 0, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 0.1;

float WindVar = 45.0;
float WindStrength = 2.0;
  
texture Texture;
sampler2D textureSampler = sampler_state {
    Texture = (Texture);
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

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
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 Normal : TEXCOORD0;
    float2 TextureCoordinate : TEXCOORD1;
};


float Wrap(float val, float min, float max)
{
	if (val > max)
		val = val - (max - min);
	if (val < min)
		val = val + (max - min);
	return val;
}

float ToRad(float val)
{
	return val * TORAD;
}
 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	
    float4 worldPosition = mul(input.Position, World);
	
	float cosIt = 0;
	float sinIt = 0;

	sincos(ToRad(WindVar), sinIt, cosIt);

	cosIt = cosIt * WindStrength;
	sinIt = sinIt * WindStrength;

	output.Position.x = cosIt + output.Position.x;
	output.Position.y = sinIt + output.Position.y;
	output.Position.z = sinIt + output.Position.z;

    float4 viewPosition = mul(worldPosition, View);
    output.Position= mul(viewPosition, Projection);	

	float4 diffuseColor = DiffuseColor * DiffuseIntensity;

    float4 ambient = (AmbientColor * AmbientIntensity);
    ambient.a = 1;

	float4 col = diffuseColor + ambient;
	//col.a = input.Color.a;

	output.Color = col;

    output.TextureCoordinate = input.TextureCoordinate;
	output.Normal = mul(input.Normal, World);

    return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);

    return saturate(textureColor * input.Color);
}
 
technique Textured
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}