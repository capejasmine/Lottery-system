Shader "Painter/Scribble Opaque Shader"
{
	Properties
	{
		_SourceTex ("Source Tex", 2D) = "white" {}
		_RenderTex("Render Tex",2D) = "white" {} //mask
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode",float)=0
	}
	SubShader
	{
		Tags { "IgnoreProjector"="True"}
		LOD 100
		Cull [_CullMode]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _SourceTex;
			float4 _SourceTex_ST;
			sampler2D _RenderTex;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _SourceTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_SourceTex, i.uv);
				fixed4 mask = tex2D (_RenderTex,i.uv );
				col = lerp(col,mask,mask.a);
				return col;
			}
			ENDCG
		}
	}
}
