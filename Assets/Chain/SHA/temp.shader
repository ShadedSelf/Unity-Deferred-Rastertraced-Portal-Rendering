Shader "Unlit/temp"
{
    Properties
    {

    }
    SubShader
    {
 		Tags
		{
			"RenderType"	= "Opaque"
			"Replace"		= "True"
		}

        Pass
        {
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			// sampler2D _ShadowMapTexture;

            struct v2f
            {
				float4 pos : SV_POSITION;
				// float3 normal : NORMAL;
            };

            v2f vert (appdata_full v)
            {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				// o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal).xyz);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				return 0;
  				// float doter = dot(i.normal, _WorldSpaceLightPos0.xyz);
				// float shadow = tex2D(_ShadowMapTexture, float2(i.pos.xy / _ScreenParams.xy));

                // return doter * shadow;
            }
            ENDCG
        }
    }
	FallBack "Diffuse"
}
