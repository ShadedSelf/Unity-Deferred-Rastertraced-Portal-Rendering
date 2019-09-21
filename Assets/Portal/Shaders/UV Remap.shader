
Shader "UV/UV Remap" 
{
    Properties 
	{
		_ID ("ID", Float) = -1
        _MainTex ("MainTex", 2D) = "white" {}
    }
    SubShader 
	{
		// Cull Off
        Tags 
		{
			"RenderType"	= "Opaque"
			"Replace"		= "True"
			// "ReplaceIDs"	= "True"
		}
        Pass 
		{
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" }
            
            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			float _ID;
			
            struct VertexInput 
			{
                float4 vertex : POSITION;
            };

            struct VertexOutput 
			{
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            VertexOutput vert (VertexInput v) 
			{
                VertexOutput o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = o.pos;
                return o;
            }

            float4 frag(VertexOutput i) : COLOR 
			{
                i.screenPos = float4(i.screenPos.xy / i.screenPos.w, 0, 0);
                i.screenPos.y *= _ProjectionParams.x;

                float2 uv = (1.0 - i.screenPos.xy) * -0.5 + 1.0;
                float4 color = tex2D(_MainTex, TRANSFORM_TEX(uv, _MainTex));
                return color;
            }
            ENDCG
        }
    }
    FallBack "Unlit"
}