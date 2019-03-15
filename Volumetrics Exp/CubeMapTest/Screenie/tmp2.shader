Shader "Unlit/tmp2"
{
	Properties
	{

	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				float3 ray : TEXCOORD1;
			};

			sampler2D _FrontDepth;
			sampler2D _BackDepth;
			sampler2D _CameraDepthTexture;

			int _Loops;
			int screenX;
			int screenY;
			float _Farer;
			float3 _LightPos;
			float4x4 _W2C;
			float4x4 _C2W;

			float4x4 _CameraInverseProjectionMatrix;
			float4x4 _CameraProjectionMatrixInverted;
			float4x4 _CameraProjectionMatrix;

			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = ComputeScreenPos(o.pos);

				float4 camRay = float4(v.texcoord.xy * 2 - 1, 1, 1);
				camRay = mul(_CameraInverseProjectionMatrix, camRay);
				o.ray = camRay.xyz / camRay.w;

				return o;
			}

			float2 CamSpaceToScreen(float3 pos)
			{
				float4 temp = mul(_CameraProjectionMatrix, float4(pos, 1.0));
				return temp.xy / temp.w;
			}

			float4 CamSpaceToScreen4(float3 pos)
			{
				float4 temp = mul(_CameraProjectionMatrix, float4(pos, 1.0));
				return temp;
			}

			float ScreenToZDepth(float4 pos)
			{
				return -mul(_CameraProjectionMatrixInverted, pos).z / _Farer;
			}

			float normalizeFloat(float value, float min, float max)
			{
				return (value - min) / (max - min);
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 col = 1;

				float2 terp = i.uv.xy;
				float depth = tex2Dlod(_FrontDepth, float4(i.uv.xy, 0, 0)).r;
				float depth2 = tex2Dlod(_BackDepth, float4(i.uv.xy, 0, 0)).r;
				float3 camSpaceLight = mul(_W2C, float4(_LightPos, 1)).xyz;

				float3 rayOrigin = i.ray * depth;
				float3 rayDirection = normalize(camSpaceLight - rayOrigin);
				rayDirection = _WorldSpaceLightPos0.xyz;
				float rayLength = distance(rayOrigin, camSpaceLight);
				rayLength = 100;
				float3 rayEnd = rayOrigin + rayDirection * rayLength;
				//rayEnd = camSpaceLight;

				float4 rayClipOrigin = CamSpaceToScreen4(rayOrigin);
				float4 rayClipEnd = CamSpaceToScreen4(rayEnd);

				float4 rayScreenOrigin = float4(rayClipOrigin.xyz / rayClipOrigin.w, rayClipOrigin.w);
				float4 rayScreenEnd = float4(rayClipEnd.xyz / rayClipEnd.w, rayClipEnd.w);
				float2 rayScreenDir = normalize(rayScreenEnd.xy - rayScreenOrigin.xy);

				float4 pixelPos = rayScreenOrigin * 592;
				float4 pixelEnd = rayScreenEnd * 592;


				float4 del = pixelEnd - pixelPos;
				float maxer = max(abs(del.x), abs(del.y));
				float4 temp;

				for (int i = 0; i < _Loops; i++)
				{

					float4 d6;
					d6.xy = round(lerp(0, del.xy, (float)i / maxer)) / 592;
					d6.zw = lerp(0, del.zw, (float)i / maxer) / 592;
					temp = rayScreenOrigin + d6;
					temp /= temp.w;

					float4 rayPos = lerp(rayClipOrigin, rayClipEnd, (float)i / maxer);

					float c = ScreenToZDepth(rayPos);

					if (i > maxer)
						break;

					float2 uv = rayPos.xy / rayPos.w;
					float frontDepth = tex2Dlod(_FrontDepth, float4(uv, 0, 0)).r;
					float backDepth = tex2Dlod(_BackDepth, float4(uv, 0, 0)).r;
					if (frontDepth < c && backDepth > c)
					{
						col = 0;
						break;
					}
				}

				col.a = 1;

				return col;
			}
			ENDCG
		}
	}
}