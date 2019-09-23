Shader "Deferred/PortalID"
{
	Properties
	{
		_ID ("ID", Float) = -1
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

				o.editor	= (float4)1;
				o.normals	= float4(i.normal.xyz, 1);
				o.position	= float4(i.wPos.xyz, 1);
				o.renderID	= (float4)_ID;

				return o;
			}
			ENDCG
		}
	}
}