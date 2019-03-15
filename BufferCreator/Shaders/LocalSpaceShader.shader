Shader "Shadows/LocalSpace"
{
	Properties
	{

	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" "DisableBatching" = "True" }

	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		struct v2f
		{
			float4 pos : SV_POSITION;
			float4 local : TEXCOOD0;
		};


		v2f vert(appdata_base v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
			//o.local = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
			o.local = v.vertex;

			return o;
		}

		float4 frag(v2f i) : SV_Target
		{
			return i.local;
		}
		ENDCG
		}
	}
}
