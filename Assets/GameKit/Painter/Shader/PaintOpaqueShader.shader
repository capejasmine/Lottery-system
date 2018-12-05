Shader "Painter/Paint Opaque Shader"
{
	Properties
	{
		_BaseTex ("Base Texture", 2D) = "white" {}
		_MainTex ("Main Texture", 2D) = "white" {}
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
				float2 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _BaseTex;
			float4 _BaseTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_BaseTex, i.uv1);
				fixed4 col1 = tex2D(_MainTex, i.uv);
				return lerp(col,col1,col1.a);
			}
			ENDCG
		}
	}
}
