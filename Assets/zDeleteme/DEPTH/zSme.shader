Shader "Unlit/zSme"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "DisableBatching " = "True"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				float4 projCoords : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				float depth : TEXCOORD2;
			};

			sampler2D _LightDepthMap;
			sampler3D _Volume;
			StructuredBuffer<float4x4> _B;

			float4x4 _iLTWF;
			float3 _LightPos;
			float _FarClip;
			float _POW;
			
			v2f vert (appdata_full v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1));
				o.normal = normalize(mul(unity_ObjectToWorld, v.normal));
				o.depth = COMPUTE_DEPTH_01;
				o.depth = -UnityObjectToViewPos(v.vertex).z;

				o.projCoords = mul(_B[0], o.worldPos);
				//o.projCoords = mul(unity_WorldToShadow[0], o.worldPos);


				return o;
			}

			inline float3 rand(float3 p)
			{
				p = float3(dot(p, float3(127.1, 311.7, 475.6)), dot(p, float3(269.5, 676.5, 475.6)), dot(p, float3(318.5, 183.3, 713.4)));
				return frac(sin(p) * 43758.5453);
			}

			inline fixed4 getCascadeWeights(float3 wpos, float z)
			{
				fixed4 zNear = float4(z >= _LightSplitsNear);
				fixed4 zFar = float4(z < _LightSplitsFar);
				fixed4 weights = zNear * zFar;
				return weights;
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
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = 1;

				float fragDepth = i.projCoords.z;
				float lightDepth = tex2Dlod(_LightDepthMap, float4(i.projCoords.xy / i.projCoords.w, 0, 0)).x;

				float d = dot(_WorldSpaceLightPos0.xyz, i.normal);
				col = d * .5 + .5;


				/*float4 cascadeWeights = getCascadeWeights(i.worldPos, i.depth);
				//float4 cascadeWeights = getCascadeWeights_splitSpheres(i.worldPos);
				float4 shadowCoord = getShadowCoord(i.worldPos, cascadeWeights);

				fragDepth = shadowCoord.z;
				lightDepth = tex2D(_LightDepthMap, shadowCoord.xy / shadowCoord.w);*/

				if (lightDepth > fragDepth + 0 && d > 0)
					col *= .5;

				float df = pow(i.depth, 1 / _POW);
				float3 uvs = float3(i.pos.xy / _ScreenParams.xy, df);
				uvs += (rand(float3(i.pos.xy / _ScreenParams.xy, df) * _Time.x) * 2 - .5) * .005;

				float sha = tex3D(_Volume, uvs).x;

				col = sha;

				col = (i.depth - _ProjectionParams.y) / (_ProjectionParams.z - _ProjectionParams.y);


				return col;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
