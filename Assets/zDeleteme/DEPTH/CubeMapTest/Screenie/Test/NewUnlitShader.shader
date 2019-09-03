Shader "Unlit/NewUnlitShader"
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

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 col : TEXCOORD0;
			};
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.col = v.vertex.xyz;

				return o;
			}

			float4 _B[10];
			float _N;
			
			fixed4 frag (v2f i) : SV_Target
			{	
				float3 col = abs(i.col) * 2;
				
				float3 t = sin(col * 20) + (sin(col * 10) * 100) + sin(col * 60) * 60;

				t = 0;
				for(int i = 0; i < _N; ++i)
				{
					t += sin(col * _B[i].x) * _B[i].y;
				}

				col = (col > t) ? 0 : 1;
				float m = ((col.x + col.y + col.z) < 2) ? 0 : 1;

				return m;
			}
			ENDCG
		}
	}
}
