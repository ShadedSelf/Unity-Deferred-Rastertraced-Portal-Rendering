Shader "Shadows/DepthTest"
{
	Properties
	{

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Cull Front

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include  "RenderThings.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 depth : TEXCOORD0;
			};

			samplerCUBE _LightDepthTex;

			float4x4 me_WorldToCamera;
			float _FarClip;

			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				float4 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1));
				o.depth = mul(me_WorldToCamera, worldPos);

				return o;
			}
			
			float1 frag (v2f i) : SV_Target
			{
				float3 depthNormal = HomoToAxis(i.depth.xyz);
				float lightDepth = length(depthNormal * i.depth.xyz) / _FarClip;

				return lightDepth;
			}
			ENDCG
		}
	}
}
