Shader "Unlit/A"
{
	Properties
	{
		_MainTex ("Texture", 3D) = "white" {}
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
				float2 uv : TEXCOORD0;
				float3 ee : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float depth : TEXCOORD2;
			};

			sampler3D _MainTex;

			float4 _Origins[4];
			float4 _Directions[4];

			float4x4 _LTWF;
			float _POW;
			float _EdgeDis;
			
			v2f vert (appdata_base v)
			{
				v2f o;

				o.ee = v.vertex.xyz + .5;

				/*v.vertex.z = pow(v.vertex.z + .5, _POW) - .5;
				v.vertex *= 2;

				v.vertex = mul(_LTWF, float4(v.vertex.xyz, 1));
				v.vertex.xyz = v.vertex.xyz / v.vertex.w;
				v.vertex.w = 1;*/

				float3 O = lerp(lerp(_Origins[2], _Origins[3], o.ee.x), lerp(_Origins[0], _Origins[1], o.ee.x), o.ee.y);
				float3 D = lerp(lerp(_Directions[2], _Directions[3], o.ee.x), lerp(_Directions[0], _Directions[1], o.ee.x), o.ee.y);
				float z = o.ee.z;
				z = pow(z, _POW);
				//z = exp(z - 1);
				v.vertex.xyz = O + D * z * _EdgeDis;



				//o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex = mul(UNITY_MATRIX_VP, v.vertex);
				o.uv = v.texcoord;
				o.depth = COMPUTE_DEPTH_01;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex3D(_MainTex, i.ee).x;
				//return float4(i.ee, 1);

				return col;
			}
			ENDCG
		}
	}
}
