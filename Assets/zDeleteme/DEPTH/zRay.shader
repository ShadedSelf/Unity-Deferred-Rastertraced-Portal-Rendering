Shader "Hidden/zRay"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		//Blend One SrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};


			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			sampler2D _MainTex;			
			sampler2D _WorldPos;
			sampler2D _LightDepthMap;
			sampler2D _Noise;

			StructuredBuffer<float4x4> _B;
			float4 _Origins[4];
			int _Steps;
			float _POW;

			float rand(float2 co) {
				return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
			}

			float3 rand(float3 p)
			{
				p = float3(dot(p, float3(127.1, 311.7, 475.6)), dot(p, float3(269.5, 676.5, 475.6)), dot(p, float3(318.5, 183.3, 713.4)));
				return frac(sin(p) * 43758.5453);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float4 ac = tex2D(_MainTex, i.uv.xy);

				//float4 col = Linear01Depth(tex2D(_CameraDepthTexture, i.uv).r);
				float3 pos = tex2D(_WorldPos, i.uv).xyz;

				float3 camPos = lerp(lerp(_Origins[2], _Origins[3], i.uv.x), lerp(_Origins[0], _Origins[1], i.uv.x), i.uv.y).xyz;

				float dis = distance(pos, camPos);
				dis = tex2D(_WorldPos, i.uv).w;

				float3 dir = normalize(camPos - pos);

				float4 col = 0;

				for (int i = 0; i < _Steps; i++)
				{
					float3 p = lerp(camPos, pos, pow(i / (float)_Steps, _POW));
					p += (rand(p * _Time.x) * 2 - .5) * .2 * dis / 40;

					float4 projCoords = mul(_B[0], float4(p.xyz, 1));

					float fragDepth = projCoords.z;
					float lightDepth = tex2Dlod(_LightDepthMap, float4(projCoords.xy / projCoords.w /*+ ((rand(p * _Time.x).xy * 2 - .5) * .001)*/, 0, 0)).x;

					if (lightDepth < fragDepth)
						col += 1.0 / _Steps /* (dot(-dir, _WorldSpaceLightPos0) * .5 + .5)*/;
				}

				col += (rand(float2(pos.x, dir.z) * _Time.x) * 2 - .5) * .008;

				if (pos.x == 0 && pos.y == 0 && pos.z == 0)
					col = 1;
				//col = (ac * .25) + (col * .75);
				col = (ac + col * 4) / 5;


				return col;
			}
			ENDCG
		}
	}
}
