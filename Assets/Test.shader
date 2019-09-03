// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

    
Shader "Test/Test"
{
	Properties
	{
		_ID ("ID", Float) = -1 //Needed?
	}
	SubShader
	{
		Tags
		{ 
			"Replace"			= "True"
			// "ReplaceIDs"		= "True"
			"DisableBatching"	= "True"
		}
		// Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float _ID;
			float4x4 portalTransform;

			// struct appdata
			// {
			// 	float4 vertex : POSITION;
			// 	float2 uv : TEXCOORD0;
			// };

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : TEXCOORD0;
				float3 wPos : TEXCOORD1;
			};

			struct f2o 
			{
				float4 col0 : COLOR0;
				float4 col1 : COLOR1;
				// float4 col2 : COLOR2;
			};

			v2f vert (appdata_full v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				float3 normal = mul(unity_ObjectToWorld, float4(v.normal.xyz, 0)).xyz;
				normal = mul(portalTransform, float4(normal.xyz, 0)).xyz;
				o.normal = normal;

				float3 wPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
				wPos = mul(portalTransform, float4(wPos.xyz, 1)).xyz;
				o.wPos = wPos;

				return o;
			}
			
			sampler2D _MaiTex;

			f2o frag (v2f i)
			{
				f2o o;
				o.col0 = (float4)_ID;
				o.col1 = float4(i.normal.xyz + i.wPos.xyz, 1);
				// o.col1 = float4(i.wPos.xyz, 1);
				// o.col2 = float4(0, 0, 1, 1);
				return o;
			}
			ENDCG
		}
	}
}