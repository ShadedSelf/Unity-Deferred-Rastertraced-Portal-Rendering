Shader "Hidden/Edgy"
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

			sampler2D _WorldSpace;
			sampler2D _EdgeBuffer;

			float _PosBias;

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

			float4 frag(v2f i) : SV_Target
			{
				float4 now = tex2D(_EdgeBuffer, i.uv);
				float4 worldSpace = tex2D(_WorldSpace, i.uv);
				uint2 pixel = floor(i.uv * _ScreenParams.xy);

				float t = 1;
				int3 check = 0;
				check += (abs(now.xyz - Neig(_EdgeBuffer, i.uv, int2(1, 0)).xyz) > _PosBias) ? 1 : 0;
				check += (abs(now.xyz - Neig(_EdgeBuffer, i.uv, int2(0, 1)).xyz) > _PosBias) ? 1 : 0;
				check += (abs(worldSpace.xyz - Neig(_WorldSpace, i.uv, int2(1, 0)).xyz) > _PosBias) ? 1 : 0;
				check += (abs(worldSpace.xyz - Neig(_WorldSpace, i.uv, int2(0, 1)).xyz) > _PosBias) ? 1 : 0;
				if (check.x + check.y + check.z > 0)
					t = .1;


				clip(-t + .5);
				return float4(t, t, t, t/*worldSpace.w / 100*/);
			}
		ENDCG
		}
	}
}
