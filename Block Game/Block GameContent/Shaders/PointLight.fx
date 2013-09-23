#define MAX_LIGHTS 50

float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: add effect parameters here.

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.5;

float3 DiffuseLightDirection = float3(1, 1, 1);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 0.1;

float3 LightPosition = float3(32, 32, 48);
float4 LightDiffuseColor = float4(0.3, 0.05, 0, 1);
float LightDistance = 100;

float3 LightPositions[MAX_LIGHTS];
float4 LightDiffuseColors[MAX_LIGHTS];
float LightDistances[MAX_LIGHTS];
int lightCount = 0;

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
	float4 Color : COLOR0;
};

struct PixelShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR1;
    float3 Normal : TEXCOORD0;
    float2 TextureCoordinate : TEXCOORD1;
};


float4 CalcColorForLight(float4 Position, int lightID)
{
	return mul(LightDiffuseColors[lightID],
	saturate(1.0f - (length(mul(Position, World) - LightPositions[lightID]) / LightDistances[lightID])));
}

PixelShaderInput VertexShaderFunction(VertexShaderInput input)
{
    PixelShaderInput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position= mul(viewPosition, Projection);

	float4 tempColor = input.Color;
	[loop]
	for (int x = 0; x < lightCount; x++)
	{
		tempColor = saturate(tempColor + CalcColorForLight(input.Position, x));
	}

	//float4 normal = normalize(mul(input.Normal, World)); //World-WorldInverseTranspose
    float lightIntensity = dot(input.Normal, DiffuseLightDirection);
	float4 diffuseColor = DiffuseColor * DiffuseIntensity * lightIntensity;
	tempColor = saturate(diffuseColor + tempColor);
	tempColor.a = input.Color.a;

	output.Color = tempColor;
    output.TextureCoordinate = input.TextureCoordinate;
	output.Normal = mul(input.Normal, World);

    return output;
}

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{ 
    float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    //textureColor.a = input.;
	
    float4 ambient = (AmbientColor * AmbientIntensity);
    ambient.a = 1;

    return saturate(textureColor * ambient + input.Color);
}

technique Textured
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}