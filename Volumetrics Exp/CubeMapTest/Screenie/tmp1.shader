Shader "Unlit/tmp1"
{
	Properties
	{

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
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
			float _Farer;
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

			float ScreenToZDepth(float4 pos)
			{
				return -mul(_CameraProjectionMatrixInverted, pos).z / _Farer;
			}

			bool CheckColl(float2 uv, float depth)
			{
				float frontDepth = tex2Dlod(_FrontDepth, float4(uv, 0, 0)).r;
				float backDepth = tex2Dlod(_BackDepth, float4(uv, 0, 0)).r;
				if(frontDepth < depth - .001 && backDepth > depth)
					return true;
				return false;
			}

			float normalizeFloat(float value, float min, float max)
			{
				return (value - min) / (max - min);
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float4 col = 1;

				float2 terp = i.uv.xy;
				float depth = tex2Dlod(_FrontDepth, float4(i.uv.xy, 0, 0)).r;
				float depth2 = tex2Dlod(_BackDepth, float4(i.uv.xy, 0, 0)).r;
				float3 camSpaceLight = mul(_W2C, float4(_LightPos, 1)).xyz;

				float3 rayOrigin = i.ray * depth;
				float3 rayDirection = normalize(camSpaceLight - rayOrigin);
				float rayLength = distance(rayOrigin, camSpaceLight);
				float3 rayEnd = rayOrigin + rayDirection * rayLength;
				rayEnd = camSpaceLight;

				float4 rayClipOrigin = CamSpaceToScreen4(rayOrigin);
				float4 rayClipEnd = CamSpaceToScreen4(rayEnd);

				float4 rayScreenOrigin = float4(rayClipOrigin.xyz / rayClipOrigin.w, rayClipOrigin.w);
				float4 rayScreenEnd = float4(rayClipEnd.xyz / rayClipEnd.w, rayClipEnd.w);
				float2 rayScreenDir = normalize(rayScreenEnd.xy - rayScreenOrigin.xy);

				/*float2 pixelPos = floor(rayScreenOrigin.xy * 634) / (634 - 1);
				float2 pixelEnd = floor(rayScreenEnd.xy * 634) / (634 - 1);*/


				float2 pixelPos = floor(rayScreenOrigin.xy * 637);
				float2 pixelEnd = floor(rayScreenEnd.xy * 637);


				float2 dx = rayScreenEnd.xz - rayScreenOrigin.xz;
				float zx;
				if (abs(dx.x) < abs(dx.y))
				{
					zx = abs(dx.x) / abs(dx.y);
					//ter = true;
				}
				else
					zx = abs(dx.y) / abs(dx.x);


				float tererer = 0;


				float2 del = pixelEnd - pixelPos;

				float m;
				bool ter = false;

				if(del.x == 0) del.x = .0001;
				if(del.y == 0) del.y = .0001;

				if (abs(del.x) < abs(del.y))
				{
					m = abs(del.x) / abs(del.y);
					ter = true;
				}
				else
					m = abs(del.y) / abs(del.x);

				float2 st = float2(0, 0);
				float c = .5;

				float3 deb;

				for(int i = 1; i < _Loops + 1; i++)
				{				
					float2 doer = round(st * sign(del));
					if(ter)	doer.xy = doer.yx;
					if ((ter && del.x >= 0 && del.y <= 0) || (ter && del.x < 0 && del.y > 0))	doer *= -1;

					float2 temper = pixelEnd - (pixelPos + doer);
					if (sign(temper).x != sign(del).x && sign(temper).y != sign(del).y)
					{
						col = float4((float)i / _Loops, 1, 0, 1);
						break;
					}// NO

					float2 next = (pixelPos + doer) / 637;
					deb.xy = next;
					deb.z = tererer;

					st.x += 1;
					st.y += m;
					tererer += zx;

					if(next.x > 1 || next.y > 1 || next.x < 0 || next.y < 0)
					{
						col = float4(1, 0, 0, 1);
						break;
					}

					/*float t = normalizeFloat(next.x, pixelPos.x, pixelEnd.x);

					float4 rayPos = lerp(rayClipOrigin, rayClipEnd, t);
					float2 uvs = rayPos.xy / rayPos.w;
					//uvs = next;

					if(CheckColl(uvs, ScreenToZDepth(rayPos)))
					{
						col = 0;
						break;
					}
					col = float4(0, .5, 1, 1);*/
				}

				//col *= depth;
				//col = depth2 - depth;

				//col = tex2D(_FrontDepth, rayScreenOrigin.xy / rayScreenOrigin.w).r;
				//col.xyz = rayScreenDir.xyz / rayScreenDir.w / 130;

				//col = tex2D(_FrontDepth, deb).r;

				//col = ScreenToZDepth(rayClipOrigin);
				//col = depth;
				//col = ScreenToZDepth(lerp(rayClipOrigin, rayClipEnd, 0));
				//col = tererer / 637;
				//col = rayScreenOrigin.z;
				//col.xyz = deb;

				col.a = .5;

				return col;
			}
			ENDCG
		}
	}
}