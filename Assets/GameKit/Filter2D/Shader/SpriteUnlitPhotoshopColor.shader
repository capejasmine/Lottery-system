// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Game/Filter2D/Sprite Photoshop Color" 
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
	    _BlendTex ("Blend Texture", 2D) = "white" {}   //you can set a texture of alpha=0 as default
	    _Opacity ("Blend Opacity", Range(0.0, 1.0)) = 1.0  
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		 
		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "FiltersCG.cginc"

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
				float4 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;
			sampler2D _BlendTex;
			float4 _BlendTex_ST;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord.xy = IN.texcoord; 
				OUT.texcoord.zw = TRANSFORM_TEX(IN.texcoord,_BlendTex); 
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			float _AlphaSplitEnabled;
			fixed _Opacity;


			fixed4 SampleSpriteTexture (float4 uv)
			{
				fixed4 color = tex2D(_MainTex,uv.xy);
				fixed4 blendColor = tex2D(_BlendTex,uv.zw);
				color.rgb = frag_color(color,blendColor,_Opacity*blendColor.a); 

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r; 
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color; 
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
