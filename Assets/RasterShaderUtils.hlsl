
struct v2f
{
	float4 vertex	: SV_POSITION;
	float3 normal	: TEXCOORD0;
	float3 wPos		: TEXCOORD1;
};

struct f2o 
{
	float4 editor	: SV_Target0;
	float4 normals	: SV_Target1;
	float4 position	: SV_Target2;
	float4 renderID	: SV_Target3;
};