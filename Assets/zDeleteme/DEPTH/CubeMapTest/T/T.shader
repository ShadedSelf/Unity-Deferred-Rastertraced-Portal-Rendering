Shader "Unlit/T"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _FrontDepth;
			sampler2D _BackDepth;
	
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = ComputeScreenPos(o.pos);

				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float col = 1;
				float frontDepth = tex2D(_FrontDepth, i.pos.xy / _ScreenParams.xy).r;
				float backDepth = tex2D(_BackDepth, i.pos.xy / _ScreenParams.xy).r;
				col = backDepth - frontDepth;

				return col;
			}
			ENDCG
		}
	}
}
