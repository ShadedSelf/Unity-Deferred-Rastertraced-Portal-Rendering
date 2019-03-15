Shader "Shadows/EdgeBuffer"
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
				float4 edge : TEXCOOD0;
			};


			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
				o.edge = float4(v.vertex.xyz * v.normal + mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz, -UnityObjectToViewPos(v.vertex).z);
				// If diff object pos and small depth diff remove line;

				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				return i.edge;
			}
			ENDCG
		}
	}
		Fallback Off
}

