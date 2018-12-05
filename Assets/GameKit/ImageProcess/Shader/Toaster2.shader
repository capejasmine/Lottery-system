// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Game/ImageProcess/Toaster2" { 
		Properties{
			_MainTex("Main Texture", 2D) = "Transparent" {}
			_CurveTex("Beauty Curve Texture",2D) = "Transparent" {}
			_Width("width",Range(0,4096)) = 600 
			_Height("height",Range(0,1024)) = 315
			_BeautyLevel("beauty level",Range(0.3333,1)) = 0.6888

			_MetalTex("metal texture",2D) = "Transparent"{}
			_SoftLightTex("soft light texture",2D) = "Transparent"{}
			_ToasterCurveTex("toaster texture",2D) = "Transparent"{}
			_OverlayMapTex("overlap map texture",2D) = "Transparent"{}
			_ColorShiftTex("color shift texture",2D) = "Transparent"{}
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

			uniform float4 _MainTex_ST;
			sampler2D _MainTex;
			sampler2D _CurveTex;
			uniform sampler2D _MetalTex;
			uniform sampler2D _SoftLightTex;
			uniform sampler2D _ToasterCurveTex;
			uniform sampler2D _OverlayMapTex;
			uniform sampler2D _ColorShiftTex;
			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				float4 col = doBeauty(_MainTex,_CurveTex,i.texcoord);
				col =  doToaster2(col,_MetalTex,_SoftLightTex,_ToasterCurveTex,_OverlayMapTex,_ColorShiftTex,i.texcoord);
				return col;
			}

			ENDCG
		}

	}
	Fallback "Diffuse"  
}
