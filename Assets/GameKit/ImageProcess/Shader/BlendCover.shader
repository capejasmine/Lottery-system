// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Game/ImageProcess/BlendCover" { 
		Properties{
			_MainTex("Main Texture", 2D) = "black" {}
			 _Color ("Main Color", Color) = (1,1,1,1)
		}
		SubShader{
			Tags
			{ 
				"Queue"="Transparent" 
				"IgnoreProjector"="True" 
				"RenderType"="Transparent" 
			}
			Lighting Off Cull Off ZTest Always ZWrite Off Fog{ Mode Off }
			Blend One OneMinusSrcColor
//				Blend SrcAlpha OneMinusSrcAlpha

			Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};


			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};


			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			float4 _Color;
			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				float4 col = tex2D(_MainTex,i.texcoord) * _Color * 0.7 ;
				return col;
			}

			ENDCG
		}
	}
	
}
