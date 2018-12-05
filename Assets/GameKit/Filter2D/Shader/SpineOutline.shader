// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Spine/Outline" {
	Properties {
		_MainTex ("Texture to blend", 2D) = "black" {}
		_Outline("Outline",Range(0,0.05)) =0
		_OutlineColor ("Outline Color", Color) = (1,1,1,1)
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc("Src Factor",float)=5
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendDst("Dst Factor",float)=10
		[MaterialToggle] PremultiplyAlpha("Premultiply Alpha",Float) = 0
	}
	// 1 texture stage GPUs
	SubShader {
		Pass {
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

			Fog { Mode Off }
			ZWrite Off
			ZTest LEqual
			Cull Off
			Lighting Off
			Cull [_CullMode]
			Blend [_BlendSrc] [_BlendDst]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile __ PREMULTIPLYALPHA_ON
			#include "UnityCG.cginc"
			#include "FiltersCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos:SV_POSITION;
				float2  uv : TEXCOORD1;
				float4 color:COLOR;
			};

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;

			v2f vert (appdata_t v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				return o;
			}

			float _Outline;
			fixed4 _OutlineColor;

			float4 frag (v2f i) : COLOR {
				fixed4 color = frag_outline2(_MainTex, i.uv,_Outline,_OutlineColor)*i.color;//frag_outline1
				#ifdef PREMULTIPLYALPHA_ON
				color*=color.a;
				#endif
				return color;
			}
			ENDCG
		}
	}
}
