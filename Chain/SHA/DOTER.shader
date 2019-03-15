Shader "Unlit/s"
{
	Properties
	{
		_Lightness("Point Three", Range(0,1)) = 0.3
		_Div("Div", Int) = 2
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
		Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float _Lightness;
			int _Div;
			sampler2D _ShadowMapTexture;

			float4x4 me_WorldToCamera;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				float3 uv : TEXCOORD0;
				float zeta : TEXCOORD1;
			};

			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal).xyz);
				o.uv = v.texcoord;
				o.zeta = -UnityObjectToViewPos(v.vertex).z / _ProjectionParams.z;

				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 ret = 1;
				float doter = dot(i.normal, _WorldSpaceLightPos0.xyz);
				float shadow = tex2D(_ShadowMapTexture, float2(i.pos.xy / _ScreenParams.xy));

				float color = saturate(doter * .5 + .5);
				//ret.xyz = color;
				//ret.xyz += (1 - color) * _Lightness;

				uint2 pixel = floor(i.pos.xy);

				if (pixel.x % round(lerp(1, 5, color)) == 0 && pixel.y % round(lerp(1, 5, color)) == 0)
					ret = 0;




				return ret;
			}
			ENDCG
		}
	}
		FallBack "Diffuse"
}