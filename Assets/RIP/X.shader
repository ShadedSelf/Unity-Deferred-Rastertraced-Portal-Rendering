  Shader "Instanced/InstancedSurfaceShader" {
    Properties {
		_C ("_C", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        // LOD 200

        CGPROGRAM

        #pragma surface surf Standard addshadow fullforwardshadows vertex:vert
        #pragma multi_compile_instancing
        #pragma instancing_options procedural:setup
		//#pragma target 5.0

		float4 data;
		float _Res;
		float2 _Scale;
		float4 _Color;
		float4 _C;
		float3 _Pos;


        struct Input 
		{
            float2 uv_MainTex;
        };

		struct H
		{
			float4 posh;
			float4 col;
		};

    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        StructuredBuffer<H> buff;
    #endif

    void setup()
        {
        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		
            data = buff[unity_InstanceID].posh;
			_Color = buff[unity_InstanceID].col;

            unity_ObjectToWorld._11_21_31_41 = float4(1 * _Scale.x, 0, 0, 0);
            unity_ObjectToWorld._12_22_32_42 = float4(0, abs(data.w) * _Scale.y, 0, 0);
            unity_ObjectToWorld._13_23_33_43 = float4(0, 0, 1 * _Scale.x, 0);
            unity_ObjectToWorld._14_24_34_44 = float4((data.xyz + _Pos) /*+ float3(0, data.w / 2, 0)*/* float3(_Scale.x, 1, _Scale.x), 1);
            unity_WorldToObject = unity_ObjectToWorld;
            unity_WorldToObject._14_24_34 *= -1;
            unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;


        #endif
        }

		void vert (inout appdata_full v) 
		{

		}

        void surf (Input IN, inout SurfaceOutputStandard o) 
		{
            o.Albedo.rgb = float3(0, 1, 1);
			o.Albedo.g *= (data.w / _Res);
			o.Albedo = (float3)(data.w < 0) ? _C : o.Albedo;
			o.Albedo *= abs(data.w / _Res);

			//o.Albedo = float3(data.w / _Res, 0, 0);
			//o.Albedo = data.xyz / 32 + float3(0, data.w / _Res, 0);
			//o.Albedo = data.w / _Res;
			float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;
			o.Albedo = (_Color.w > .5) ? 1 : o.Albedo;

            o.Metallic = 0;
            o.Smoothness = 0;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
