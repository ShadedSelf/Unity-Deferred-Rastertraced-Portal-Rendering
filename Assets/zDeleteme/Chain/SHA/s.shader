Shader "Unlit/s"
{
	Properties
	{
		_Lightness("Point Three", Range(0,1)) = 0.3
		_Div("Div", Int) = 2
	}
	SubShader
	{
		Tags
		{
			"RenderType"	= "Opaque"
			"Replace"		= "True"
			// "ReplaceIDs"	= "True"
		}

		Pass
		{
			// Tags
			// { 
			// 	"LightMode"	= "ForwardBase"
			// 	"Replace"	= "True"
			// }

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

				float color = doter * .5 + .5;
				ret.xyz = color;
				ret.xyz += (1 - color) * _Lightness;

				uint2 pixel = floor(i.pos.xy);
				//pixel = 2;
				if (pixel.x % _Div == 0 && pixel.y % _Div == 0 && shadow < .5 && doter > .001 || (pixel.x % _Div == 0 && pixel.y % _Div == 0 && doter < 0))
					ret = lerp(0, .5 + _Lightness / 2, 1 - abs(doter));

				float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;
				//float3 psy = float3(0, sin((i.pos.x + _Time.x / 2) * i.pos.y) * .5 + .5, sin((i.pos.x + _Time.x / 4) * i.pos.y) * .5 + .5) * 1 * i.pos / _ScreenParams.xyz;
				float3 inner = float3(i.pos.xy / _ScreenParams.xy, sin(i.zeta + _Time.x) * .5 + .5) * lerp(1.1, 1, doter * .5 + .5);
				inner = float3(i.zeta, i.pos.x / _ScreenParams.x, i.pos.y / _ScreenParams.y);
				/*if (i.uv.x < .8) ret.xyz = inner;
				if (i.uv.x < .8 && pixel.x % 2 == 0 && pixel.y % 2 == 0) ret.xyz = lerp(inner, 1, doter * .5 + .5);
				if (i.uv.x < .8 && shadow < .5 && doter > .001) ret.xyz = lerp(inner, ret.xyz, 1 - abs(doter));
				*/

				return ret;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
    