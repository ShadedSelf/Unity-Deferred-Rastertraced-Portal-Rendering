Shader "Unlit/s"
{
	Properties
	{

	}
	SubShader
	{
		Tags
		{
			"RenderType"	= "Opaque"
			"Replace"		= "True"
		}

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float _Timer;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
			};

			float3x3 AngleAxis3x3(float angle, float3 axis)
			{
				float c, s;
				sincos(angle, s, c);

				float t = 1 - c;
				float x = axis.x;
				float y = axis.y;
				float z = axis.z;

				return float3x3
				(
					t * x * x + c,      t * x * y - s * z,  t * x * z + s * y,
					t * x * y + s * z,  t * y * y + c,      t * y * z - s * x,
					t * x * z - s * y,  t * y * z + s * x,  t * z * z + c
				);
			}

			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal).xyz);

				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				return 1;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
