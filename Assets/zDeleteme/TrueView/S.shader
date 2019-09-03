// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

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

			sampler2D _ShadowMapTexture;
			float4x4 me_WorldToCamera;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				float3 test : TEXCOORD0;
				// float3 uv : TEXCOORD0;
				// float zeta : TEXCOORD1;
			};

			v2f vert(appdata_full v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				// o.pos /= o.pos.w;

				float3 vPos = UnityObjectToViewPos(v.vertex).xyz;

				// float x = dot(float3(1, 0, 0), normalize(vPos * float3(1, 0, 1)));
				// x *= 8;
				// if (sign(x) != sign(vPos.x))
				// 	x = 100;

				// float y = dot(float3(0, 1, 0), normalize(vPos * float3(0, 1, 1)));
				// y *= -8;
				// if (sign(y) != sign(vPos.y))
				// 	y = 100;



				float x = vPos.x;
				float y = -vPos.y;
				// float z = -(((-vPos.z / 100) - 0.5) * 2);
				float z = 1 - (-vPos.z / 100); //Fix
				float w = -vPos.z;

				o.pos.x = x;
				o.pos.y = y;
				o.pos.z = z;
				o.pos.w = w;


				o.test.xyz = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
				// o.test.x = o.pos.z * o.pos.w;
				o.test.x = w;
				// o.test.x = -vPos.z;

				o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal).xyz);

				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				// return i.pos.z;
				// return i.test.x * 0.1;
				// return float4(i.test.xyz, 1) * 0.1;
				float doter = dot(i.normal, _WorldSpaceLightPos0.xyz) * 0.5 + 0.5;
				return doter;
			}
			ENDCG
		}
	}
	// FallBack "Diffuse"
}
