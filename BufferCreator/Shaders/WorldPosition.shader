Shader "Shadows/LocalSpace"
{
	Properties
	{

	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "DisableBatching" = "True" }
		LOD 100

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct v2f
	{
		float4 pos : SV_POSITION;
		float4 world : TEXCOOD0;
	};


	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
		o.world = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1));
		//o.world.w = -UnityObjectToViewPos(v.vertex).z;
		o.world.w = distance(_WorldSpaceCameraPos, o.world.xyz);

		return o;
	}

	float4 frag(v2f i) : SV_Target
	{
		return i.world;
	}
		ENDCG
	}
	}
}
