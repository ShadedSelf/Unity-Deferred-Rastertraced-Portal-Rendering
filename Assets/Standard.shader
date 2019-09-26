Shader "Deferred/Standard"
{
	Properties
	{
	}
	SubShader
	{
		Tags
		{ 
			"DisableBatching"	= "True"
		}
		// Cull Off ZWrite Off ZTest Always
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "RasterShaderUtils.hlsl"

			float _ID;
			float4x4 portalTransform;

			v2f vert (appdata_full v)
			{
				v2f o;
				
				o.vertex = UnityObjectToClipPos(v.vertex);

				float3 normal = mul(unity_ObjectToWorld, float4(v.normal.xyz, 0)).xyz;
				normal = mul(portalTransform, float4(normal.xyz, 0)).xyz;
				o.normal = normal;

				float3 wPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
				wPos = mul(portalTransform, float4(wPos.xyz, 1)).xyz;
				o.wPos.xyz = wPos;

				return o;
			}
			
			f2o frag (v2f i)
			{
				f2o o;

				i.normal.xyz = normalize(i.normal.xyz);
				float3 rd = normalize(_WorldSpaceCameraPos.xyz - i.wPos.xyz);
				float c = dot(rd, i.normal.xyz);
				// c = 1;

				o.editor	= float4((float3)c * 0.5 + 0.5, 1);
				o.normals	= float4(i.normal.xyz, 1);
				o.position	= float4(i.wPos.xyz, c);
				o.renderID	= (float4)0;

				return o;
			}
			ENDCG
		}
	}
}