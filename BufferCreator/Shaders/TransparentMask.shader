Shader "Shadows/TransparentMask"
{
	Properties
	{

	}
	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "DisableBatching" = "True" }
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
		o.world = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
		o.world.w = -UnityObjectToViewPos(v.vertex).z;

		return o;
	}

	float4 frag(v2f i) : SV_Target
	{
		return float4(0, i.world.w, 0, 0);
	}
		ENDCG
	}
	}
}
