Shader "Unlit/Point0"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { /*"RenderType"="Opaque" */}
		//LOD 100
		//Cull Off
		ZWrite On
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 projCoords : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 projCoords : TEXCOORD0;
			};

			sampler2D _LightMap;
			float4x4 _Full;

			float3 rand(float3 p)
			{
				p = float3(dot(p, float3(127.1, 311.7, 475.6)), dot(p, float3(269.5, 676.5, 475.6)), dot(p, float3(318.5, 183.3, 713.4)));
				return frac(sin(p) * 43758.5453);
			}
			
			v2f vert (appdata v)
			{
				v.vertex.xyz += sin(_Time.z / 2 + rand(v.vertex.xyz) * 50);
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float4 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1));
				o.projCoords = mul(_Full, worldPos);

				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float4 ret = 1;

				float fragDepth = i.projCoords.z;
				float lightDepth = tex2Dproj(_LightMap, i.projCoords).r;

				/*if (lightDepth < fragDepth - 0)
					ret.a = 0;
				else
					ret.a = .5;*/

				return float4(0, 0, 0, 1);
			}
			ENDCG
		}
	}
}
