Shader "Hidden/BlitVolume"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			inline float3 rand(float3 p)
			{
				p = float3(dot(p, float3(127.1, 311.7, 475.6)), dot(p, float3(269.5, 676.5, 475.6)), dot(p, float3(318.5, 183.3, 713.4)));
				return frac(sin(p) * 43758.5453);
			}
			
			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			sampler2D _MyDepth;
			sampler3D _Volume;

			float _POW;

			half4 frag (v2f i) : SV_Target
			{
				float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv).r);
				depth = (depth - _ProjectionParams.y) / (_ProjectionParams.z - _ProjectionParams.y);
				depth = pow(depth, 1 / _POW);
				//depth = 0;
				float3 uvs = (rand(float3(i.uv.xy, depth) * _Time.x) * 2 - .5) * .005;
				uvs = 0;
				float col = tex3D(_Volume, float3(i.uv, depth) + uvs).x;
				// col += (rand(float3(i.uv.xy, depth) * _Time.x) * 2 - .5) * .01;

				// //depth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv).r);

				// float4 ac = tex2D(_MainTex, i.uv);
				// col = lerp(col, ac, 0.5);

				return col;
			}
			ENDCG
		}
	}
}
