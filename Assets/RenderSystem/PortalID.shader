Shader "Replacements/PortalID"
{
	Properties
	{
		_ID ("ID", Float) = -1 //Needed?
	}
	SubShader
	{
		Tags
		{ 
			"Replace"			= "True"
			// "ReplaceIDs"		= "True"
			"DisableBatching"	= "True"
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float _ID;

			struct v2f
			{
				float4 pos	: SV_POSITION;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				return _ID;
			}
			
			ENDCG
		}
	}
	Fallback Off
}

