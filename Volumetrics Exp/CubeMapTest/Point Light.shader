Shader "Shadows/Point Light"
{
	Properties
	{
		//_LightDepthTex ("Texture", CUBE) = "" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include  "RenderThings.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				float4 projCoords : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				float3 lightDir : TEXCOORD2;
			};

			samplerCUBE _LightDepthTex;

			float4x4 me_WorldToCamera;
			float3 _LightPos;
			float3 _CamPos;
			float _FarClip;
			float _NearClip;
			
			v2f vert (appdata_full v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1));
				o.normal = normalize(mul(unity_ObjectToWorld, v.normal));
				o.lightDir = normalize(_LightPos - o.worldPos.xyz);
				o.projCoords = mul(me_WorldToCamera, o.worldPos);

				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float4 col = 1;
				//i.lightDir = normalize(i.lightDir);

				float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;

				float3 depthNormal = HomoToAxis(i.projCoords.xyz);

				float fragDepth = length(depthNormal * i.projCoords) / _FarClip;
				i.projCoords.z *= -1;
				float lightDepth = texCUBE(_LightDepthTex, i.projCoords.xyz).r;

				float d = dot(i.lightDir, i.normal);
				float worlDist = distance(i.worldPos, _LightPos) / _FarClip;
				col *= (saturate(dot(i.lightDir, i.normal)) * .5 + .5) * (1 - worlDist);

				float bias = .001;
				if(lightDepth <= fragDepth && d > 0)
					col *= .5;
				/*if(lightDepth <= fragDepth - .1 && d < 0)
					col *= .5;*/
				/*if(d < 0)
					col *= .5;*/
				//col *= lerp(1, .5, 1 - abs(clamp(d, -1, 0))); 

				col.a = 1;
				return col;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
