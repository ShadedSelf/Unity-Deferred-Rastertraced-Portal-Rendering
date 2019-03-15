Shader "Unlit/tmp"
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
				float4 uv : TEXCOORD0;
				float3 ray : TEXCOORD1;
			};

			sampler2D _FrontDepth;
			sampler2D _BackDepth;
			sampler2D _CameraDepthTexture;

			float3 _LightPos;
			float4x4 _W2C;
			float4x4 _C2W;

			float4x4 _CameraInverseProjectionMatrix;
			float4x4 _CameraProjectionMatrixInverted;
			float4x4 _CameraProjectionMatrix;
			
			v2f vert (appdata_full v)
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
				float4 temp = mul( _CameraProjectionMatrix, float4(pos, 1.0));
				return temp.xy / temp.w;
			}

			float4 CamSpaceToScreen4(float3 pos)
			{
				float4 temp = mul(_CameraProjectionMatrix, float4(pos, 1.0));
				return temp;
			}

			bool CheckColl(float3 pos, float depth)
			{
				float2 uv = CamSpaceToScreen(pos);
				float frontDepth = tex2Dlod(_FrontDepth, float4(uv, 0, 0)).r;
				float backDepth = tex2Dlod(_BackDepth, float4(uv, 0, 0)).r;
				if(frontDepth < depth && backDepth > depth)
					return true;
				return false;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float4 col = 1;

				float depth = tex2Dlod(_FrontDepth, float4(i.uv.xy, 0, 0)).r;
				float depth2 = tex2Dlod(_BackDepth, float4(i.uv.xy, 0, 0)).r;
				float3 camSpaceLight = mul(_W2C, float4(_LightPos, 1)).xyz;
				float3 rayOrigin = i.ray * depth;
				float3 rayDirection = normalize(camSpaceLight - rayOrigin);

				float rayLength = distance(rayOrigin, camSpaceLight);
				for(int i = 0; i < 100; i++)
				{
					float3 rayPos = rayOrigin + rayDirection * rayLength / 100 * i;
					if(CheckColl(rayPos, -rayPos.z / 100))
					{
						col = 0;
						break;
					}
				}

				col *= depth;
				//col = depth2 - depth;

				return col;
			}
			ENDCG
		}
	}
}
