// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Game/ImageProcess/Smooth" { 
		Properties{
			_MainTex("Main Texture", 2D) = "Transparent" {}
			_Width("width",Range(0,4096)) = 600 
			_Height("height",Range(0,1024)) = 315
			_BeautyLevel("beauty level",Range(0.3333,1)) = 0.6888
		}


		SubShader{
			Tags{
				"IgnoreProjector" = "True"
				"PreviewType" = "Plane"
			}
			Lighting Off Cull Off ZTest Always ZWrite Off Fog{ Mode Off }


			Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "ImageProcess.cginc"

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

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				float4 col = doSmooth(_MainTex,i.texcoord);
				return col;
			}

			ENDCG
		}

	}
	Fallback "Diffuse"  
}
