
cbuffer globals
{
    matrix finalMatrix;
	float costime;
	float lineartime;
}
Texture2D myTexture;

SamplerState currentSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
	WrapS = Wrap;
    //AddressV = Wrap;
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
	float4 pos = mul(position, finalMatrix);
	ouput.Pos = pos;
	ouput.UV = uv;
	ouput.ModPos = position;
	
	ouput.UV[0] = ouput.UV[0] + lineartime;
	/*
	if (ouput.UV[0]  > 1.0) {
		ouput.UV[0] = ouput.UV[0] - 1.0;
	}
	*/
	
	return ouput;
}

float4 PShader(VS_OUTPUT input) : SV_Target
{
	float2 temp = float2(input.UV[0],input.UV[1]);
	return myTexture.Sample(currentSampler, temp);	
}