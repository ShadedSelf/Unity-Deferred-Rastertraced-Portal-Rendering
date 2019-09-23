Shader "Replacements/FrontDepth"
{
	Properties
	{

	}
	SubShader
	{
		Tags
		{ 
			"Replace" = "True"
		}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float depth : TEXCOORD0;
			};

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));		
				o.depth = COMPUTE_DEPTH_01;

				return o;
			}
			
			float frag (v2f i) : SV_Target
			{
				return i.depth;
			}
			ENDCG
		}
	}
}
