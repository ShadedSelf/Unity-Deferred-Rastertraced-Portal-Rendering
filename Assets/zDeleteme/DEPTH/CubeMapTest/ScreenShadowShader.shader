Shader "Unlit/ScreenShadowShader"
{
	Properties
	{

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
			#include  "RenderThings.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 worldPos : TEXCOORD0;
				float4 projCoords : TEXCOORD1;
			};

			samplerCUBE _LightDepthTex;

			float4x4 me_WorldToCamera;
			float _FarClip;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1));
				o.projCoords = mul(me_WorldToCamera, o.worldPos);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float col = 1;

				float3 depthNormal = HomoToAxis(i.projCoords.xyz);
				float fragDepth = length(depthNormal * i.projCoords) / _FarClip;
				
				i.projCoords.z *= -1;
				float lightDepth = texCUBE(_LightDepthTex, i.projCoords.xyz).r;

				if(lightDepth <= fragDepth)
					col = 0;

				return col;
			}
			ENDCG
		}
	}
}
