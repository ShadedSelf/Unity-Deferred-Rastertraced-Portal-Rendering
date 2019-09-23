// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/z"
{
	Properties
	{
		_MainTex ("Texture", CUBE) = "white" {}
		_Distance("Distance", Float) = 0
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
				float4 vertex : SV_POSITION;
				float3 ob : TEXCOORD0;
				float3 cancel : TEXCOORD1;
			};

			samplerCUBE _MainTex;
			float _Distance;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				float3 normal = normalize(mul(unity_ObjectToWorld, float4(v.normal.xyz, 0)).xyz);
				float3 view = normalize(mul(unity_ObjectToWorld, float4(-UNITY_MATRIX_IT_MV[2].xyz, 0)).xyz);
				float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
				float dis = distance(worldPos, _WorldSpaceCameraPos);
				float3 vec = v.vertex.xyz + v.normal.xyz * dis * 2;
				float3 ref = reflect(view, normal);
				ref = mul((float3x3)unity_WorldToObject, ref);

				o.ob = vec + ref * 2;
				o.cancel = (float3)1 - abs(v.normal.xyz);

				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float4 col = texCUBE(_MainTex, i.ob);

				/*float3 canceled = abs(i.ob) * i.cancel;

				float2 uv;

				if (canceled.x > 0 && canceled.y > 0)
					uv.xy = canceled.xy * sign(i.ob.xy);
				if (canceled.x > 0 && canceled.z > 0)
					uv.xy = canceled.xz * sign(i.ob.xz);
				if (canceled.y > 0 && canceled.z > 0)
					uv.xy = canceled.yz * sign(i.ob.yz);

				uv *= 10;


				int2 pixel = floor(uv);

				col = 1;
				if (pixel.x % 2 == 0 && pixel.y % 2 == 0)
					col = 0;*/
					
				return col;
			}
			ENDCG
		}
	}
}
