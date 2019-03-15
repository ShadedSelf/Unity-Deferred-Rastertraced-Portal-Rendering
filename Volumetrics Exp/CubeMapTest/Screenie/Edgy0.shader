Shader "Hidden/Edgy0"
{
	SubShader
	{
		ZWrite Off
		Cull Off
		Lighting Off
		Blend One SrcAlpha

		Pass
		{
			CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		sampler2D _EdgeBuffer;

		float _PosBias;
		float tempW;

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			return o;
		}

		float4 Neig(sampler2D tex, float2 uvs, int2 pos)
		{
			return tex2Dlod(tex, float4(uvs + float2(pos) / _ScreenParams.xy, 0, 0));
		}

		int Checker(float3 now, float2 uv)
		{
			int3 check = 0;
			int des = 0;
			int des0 = 0;
			float4 tempNow = Neig(_EdgeBuffer, uv, int2(1, 0));
			tempW = min(tempW, (tempNow.w < .0001) ? _ProjectionParams.z : tempNow.w);
			check += (abs(now.xyz - tempNow.xyz) > _PosBias) ? 1 : 0;
			if (check.x + check.y + check.z > 0)
			{
				des = 1;
				check = 0;
			}
			tempNow = Neig(_EdgeBuffer, uv, int2(0, 1));
			tempW = min(tempW, (tempNow.w < .0001) ? _ProjectionParams.z : tempNow.w);
			check += (abs(now.xyz - tempNow.xyz) > _PosBias) ? 1 : 0;
			if (check.x + check.y + check.z > 0)
			{
				des0 = 1;
				check = 0;
			}
			if (des > 0 && des0 > 0)
				return 1;
			if (des == 0 && des0 > 0)
				return 2;
			if (des > 0 && des0 == 0)
				return 3;

			return 0;
		}

		float4 frag(v2f i) : SV_Target
		{
			float4 now = tex2D(_EdgeBuffer, i.uv);
			uint2 pixel = floor(i.uv * _ScreenParams.xy);
			float t = 1;
			tempW = now.w;
			tempW = (tempW < .0001) ? _ProjectionParams.z : tempW;

			int value = Checker(now.xyz, i.uv);

			if (value == 0 || (value == 2 && (Checker(Neig(_EdgeBuffer, i.uv, int2(0, 1)).xyz, i.uv + float2(0, 1) / _ScreenParams.xy) == 3)))
				clip(-1);

			/*if (value == 1)
				return float4(0, 0, 1, 0);
			if (value == 2)
				return float4(0, 1, 0, 0);
			if (value == 3)
				return float4(1, 0, 1, 0);*/

			return float4((float3)0, tempW / _ProjectionParams.z);
		}
			ENDCG
		}
	}
}
