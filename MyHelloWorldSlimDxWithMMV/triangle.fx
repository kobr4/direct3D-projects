
cbuffer globals
{
    matrix finalMatrix;
	float costime;
}
Texture2D myTexture;

SamplerState currentSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VS_OUTPUT
{
    float4 Pos 	: SV_POSITION;
    float2 UV	: TEXCOORD;
};

VS_OUTPUT VShader(float4 position : POSITION, float2 uv : TEXCOORD)
{
	VS_OUTPUT ouput;
	float amplitude = 0.05f;
	float4 tmppos = position;
	
	tmppos[0] = tmppos[0]+ (costime * amplitude * (uv[1]));
	float4 pos = mul(float4(tmppos), finalMatrix);
	ouput.Pos = pos;
	ouput.UV = uv;
	return ouput;
}

float4 PShader(VS_OUTPUT input) : SV_Target
{
	float2 temp = float2(input.UV[0],input.UV[1]);
	//return myTexture.Sample(currentSampler, temp);
	return float4(1.0f, 1.0f, 0.0f, 1.0f);	

}