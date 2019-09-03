Shader "Shadows/LocalNormal"
{
	Properties
	{

	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct v2f
	{
		float4 pos : SV_POSITION;
		float3 local : TEXCOOD0;
	};


	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
		o.local = v.normal;

		return o;
	}

	float4 frag(v2f i) : SV_Target
	{
		return float4(i.local, 1);
	}
		ENDCG
	}
	}
}

