float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color    : COLOR0; 
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color    : COLOR0; 
};
 
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	output.Color = input.Color;

	return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return input.Color;
}
 
technique Technique1
{
    pass Pass1
    {   
        VertexShader = compile vs_1_1 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}