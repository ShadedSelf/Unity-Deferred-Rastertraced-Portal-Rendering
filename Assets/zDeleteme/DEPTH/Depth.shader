Shader "Replacement/Depth"
{
	Properties
	{

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" /*"DisableBatching " = "True" */}

		Cull Front

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float depth : TEXCOORD0;
			};

			float4x4 _Full;

			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.depth = 1 - o.vertex.z;

				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				return i.depth;
			}
			ENDCG
		}
	}
}
