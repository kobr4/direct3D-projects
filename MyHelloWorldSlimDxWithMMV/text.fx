
cbuffer globals
{
    matrix finalMatrix;
	float costime;
}
Texture2D myTexture;

SamplerState currentSampler
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU = CLAMP;
	AddressV = CLAMP;
};

struct VS_OUTPUT
{
    float4 Pos 	: SV_POSITION;
    float2 UV	: TEXCOORD;
	float4 ModPos : POSITION;
};

VS_OUTPUT VShader(float4 position : POSITION, float2 uv : TEXCOORD)
{
	VS_OUTPUT ouput;
	float4 tmppos = position;
	
	float4 pos = tmppos;
	ouput.Pos = pos;
	ouput.UV = uv;
	ouput.ModPos = position;
	return ouput;
}

float4 PShader(VS_OUTPUT input) : SV_Target
{
	//float2 temp = float2(input.UV[0],input.UV[1]);
	return myTexture.Sample(currentSampler, input.UV);
	//return float4(1.0f, 1.0f, 0.0f, 1.0f);		
}