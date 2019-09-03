Shader "Hidden/zRayCascade"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
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


			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			sampler2D _MainTex;

			sampler2D _WorldPos;
			StructuredBuffer<float4x4> _B;
			sampler2D _LightDepthMap;
			float4 _Origins[4];
			int _Steps;
			float _POW;

			inline fixed4 getCascadeWeights(float3 wpos, float z)
			{
				fixed4 zNear = float4(z >= _LightSplitsNear);
				fixed4 zFar = float4(z < _LightSplitsFar);
				fixed4 weights = zNear * zFar;
				return weights;
			}

			inline float4 getShadowCoord(float4 wpos, fixed4 cascadeWeights)
			{
				float3 sc0 = mul(unity_WorldToShadow[0], wpos).xyz;
				float3 sc1 = mul(unity_WorldToShadow[1], wpos).xyz;
				float3 sc2 = mul(unity_WorldToShadow[2], wpos).xyz;
				float3 sc3 = mul(unity_WorldToShadow[3], wpos).xyz;
				float4 shadowMapCoordinate = float4(sc0 * cascadeWeights[0] + sc1 * cascadeWeights[1] + sc2 * cascadeWeights[2] + sc3 * cascadeWeights[3], 1);
			#if defined(UNITY_REVERSED_Z)
				float  noCascadeWeights = 1 - dot(cascadeWeights, float4(1, 1, 1, 1));
				shadowMapCoordinate.z += noCascadeWeights;
			#endif
				return shadowMapCoordinate;
			}

			inline fixed4 getCascadeWeights_splitSpheres(float3 wpos)
			{
				float3 fromCenter0 = wpos.xyz - unity_ShadowSplitSpheres[0].xyz;
				float3 fromCenter1 = wpos.xyz - unity_ShadowSplitSpheres[1].xyz;
				float3 fromCenter2 = wpos.xyz - unity_ShadowSplitSpheres[2].xyz;
				float3 fromCenter3 = wpos.xyz - unity_ShadowSplitSpheres[3].xyz;
				float4 distances2 = float4(dot(fromCenter0, fromCenter0), dot(fromCenter1, fromCenter1), dot(fromCenter2, fromCenter2), dot(fromCenter3, fromCenter3));
				fixed4 weights = float4(distances2 < unity_ShadowSplitSqRadii);
				weights.yzw = saturate(weights.yzw - weights.xyz);
				return weights;
			}

			float rand(float2 co) {
				return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
			}

			float3 rand(float3 p)
			{
				p = float3(dot(p, float3(127.1, 311.7, 475.6)), dot(p, float3(269.5, 676.5, 475.6)), dot(p, float3(318.5, 183.3, 713.4)));
				return frac(sin(p) * 43758.5453);
			}

			fixed4 frag(v2f i) : SV_Target
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

					//float4 cascadeWeights = getCascadeWeights(i.worldPos, i.depth);
					float4 cascadeWeights = getCascadeWeights_splitSpheres(p);
					float4 shadowCoord = getShadowCoord(float4(p, 1), cascadeWeights);

					float4 projCoords = mul(_B[0], float4(p.xyz, 1));
					projCoords = shadowCoord;

					float fragDepth = projCoords.z;
					float lightDepth = tex2Dlod(_LightDepthMap, float4(projCoords.xy / projCoords.w /*+ ((rand(p).xy - .5) * .001)*/, 0, 0)).x;

					if (lightDepth < fragDepth)
						col += 1.0 / _Steps /* (dot(-dir, _WorldSpaceLightPos0) * .5 + .5)*/;
				}

				//col += (rand(float2(pos.x, dir.z) * _Time.x) * 2 - .5) * .05;

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
