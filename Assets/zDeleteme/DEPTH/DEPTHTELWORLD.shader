Shader "Unlit/DEPTHTELWORLD"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float4x4 _LTWF;

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 world : TEXCOORD0;
				float4 test : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				v.vertex = mul(_LTWF, float4(v.vertex.xyz * 2, 1));
				v.vertex.xyz = v.vertex.xyz / v.vertex.w;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.world = mul(unity_ObjectToWorld, v.vertex);
				o.test = v.vertex;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return i.world;
				return i.test;
			}
			ENDCG
		}
	}
}
