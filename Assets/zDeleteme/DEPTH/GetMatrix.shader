Shader "Hidden/GetMatrix" 
{
	Properties
	{
		_MainTex("", any) = "" {}
	}
	SubShader 
	{
		Pass
		{
			ZWrite Off ZTest Always Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0

			#include "UnityCG.cginc"


			struct appdatar
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2fr 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};


			sampler2D _MainTex;

			RWStructuredBuffer<float4x4> _MatrixBuf : register(u1);

			v2fr vert(appdatar v)
			{
				v2fr o;
				//hack to make quad draw fullscreen - just convert UV into n. device coordinates
				o.pos = float4(float2(v.texcoord.x, 1.0f - v.texcoord.y) * 2.0f - 1.0f, 0.0f, 1.0f);
				o.uv = v.texcoord;
				return o;
			}


			float4 frag(v2fr i) : SV_Target
			{
				_MatrixBuf[0] = unity_WorldToShadow[0];
				/*_MatrixBuf[1] = unity_WorldToShadow[1];
				_MatrixBuf[2] = unity_WorldToShadow[2];
				_MatrixBuf[3] = unity_WorldToShadow[3];

				_MatrixBuf[4] = float4x4(_LightSplitsNear, _LightSplitsFar, float4(0, 0, 0, 0), float4(0, 0, 0, 0));*/

				return 0.0f;
			}
			ENDCG
		}
	}
	Fallback Off
}