Shader "Shadows/BackDepth"
{
	Properties
	{

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Front

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 depth : TEXCOORD0;
			};

			float3 _LightPos;

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));		
				o.depth = float4( 0.0, 0.0, COMPUTE_DEPTH_01, 0.0);

				return o;
			}
			
			float frag (v2f i) : SV_Target
			{
				return i.depth.z;
			}
			ENDCG
		}
	}
}
