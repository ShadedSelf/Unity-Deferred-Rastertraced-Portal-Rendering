Shader "Unlit/Point"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "DisableBatching" = "True" }
		LOD 100
		//Blend SrcAlpha OneMinusSrcAlpha
		//Cull Back

		Pass
		{
		Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"


			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
				float s : TEXCOORD1;
			};

			sampler2D _ShadowMapTexture;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.s = tex2Dlod(_ShadowMapTexture, float4(o.pos.xy / _ScreenParams.xy, 0, 0));

				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float4 col = float4(1, 1, 1, 1);
				float shadow = tex2D(_ShadowMapTexture, float2(i.pos.xy / _ScreenParams.xy));
				col = shadow;

				/*if (shadow < .5)
					clip(-1);*/


				return col;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
