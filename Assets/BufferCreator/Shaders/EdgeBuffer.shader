Shader "Replacements/EdgeBuffer"
{
	Properties
	{

	}
	SubShader
	{
		Tags
		{ 
			"Replace"			= "True"
			// "ReplaceIDs"		= "True"
			"DisableBatching"	= "True"
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float4x4 portalTransform;

			struct v2f
			{
				float4 pos		: SV_POSITION;
				float3 normal	: TEXCOOD0;
				float3 worldPos	: TEXCOOD1;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				// o.edge = float4(v.vertex.xyz * v.normal + mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz, -UnityObjectToViewPos(v.vertex).z); //Original
				o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));

				float3 wPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
				wPos = mul(portalTransform, float4(wPos.xyz, 1)).xyz;
				o.worldPos = wPos;


				float3 test = mul(unity_ObjectToWorld, float4(v.normal.xyz, 0)).xyz;
				test = mul(portalTransform, float4(test.xyz, 0)).xyz;
				o.normal = test;


				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				return float4(normalize(i.normal.xyz) + i.worldPos, 1);
				// return float4(normalize(i.normal.xyz), 1);
			}
			
			ENDCG
		}
	}
	Fallback Off
}

