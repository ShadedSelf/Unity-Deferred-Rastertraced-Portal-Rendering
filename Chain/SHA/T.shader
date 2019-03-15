Shader "Unlit/T"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100
		Blend One SrcAlpha
		Zwrite Off
		//Ztest Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));

				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				int2 pixel = floor(i.vertex.xyz);
				if (pixel.x % 2 == 0 || pixel.y % 2 == 0)
					clip(-1);

				float d = dot(i.normal, _WorldSpaceLightPos0.xyz);

				float4 col = float4(d.xxx * .1, .5);

				return col;
			}
			ENDCG
		}
	}
}
