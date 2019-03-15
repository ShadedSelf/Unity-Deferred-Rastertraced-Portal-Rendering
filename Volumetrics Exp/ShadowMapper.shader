Shader "Hidden/ShadowMapper"
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
			#pragma target 5.0
			
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
			
			Texture2D _MainTex;
			SamplerState sampler_MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				//fixed4 col = tex2D(_MainTex, i.uv);
				float4 col = 0;
				col += exp(_MainTex.GatherRed(sampler_MainTex, i.uv, int2(0, 0)) * 40);
				col += exp(_MainTex.GatherRed(sampler_MainTex, i.uv, int2(2, 0)) * 40);
				col += exp(_MainTex.GatherRed(sampler_MainTex, i.uv, int2(0, 2)) * 40);
				col += exp(_MainTex.GatherRed(sampler_MainTex, i.uv, int2(2, 2)) * 40);

				col = dot(col, 1 / 16.0);

				return col;
			}
			ENDCG
		}
	}
}
