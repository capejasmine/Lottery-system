// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Particle2D/Simple"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Tint ("Tint Color",Color)=(1,1,1,1)
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc("Src Factor",float)=5
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendDst("Dst Factor",float)=10
		[Enum(UnityEngine.Rendering.CullMode)] _CullMode("Cull Mode",float)=0
	}
	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
		}

		Cull [_CullMode]
		Lighting Off
		ZWrite Off
		Blend [_BlendSrc] [_BlendDst]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};

			fixed4 _Tint;
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color *_Tint;
				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = tex2D(_MainTex, IN.texcoord)*IN.color;
				return color;
			}
		ENDCG
		}
	}
}
