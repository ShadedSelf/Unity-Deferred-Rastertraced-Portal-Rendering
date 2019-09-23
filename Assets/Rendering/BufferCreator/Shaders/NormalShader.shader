Shader "Unlit/NormalShader"
{
	Properties
	{

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
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
				float3 normal : NORMAL;
			};

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = normalize(mul(unity_ObjectToWorld, v.normal));

				return o;
			}
			
			fixed3 frag (v2f i) : SV_Target
			{
				return normalize(i.normal);
			}
			ENDCG
		}
	}
}
