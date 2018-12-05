// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Game/Filter2D/Sprite Box Filter"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
        _TexSizeX("Texture SizeX", range(1,2048)) = 256//size  
        _TexSizeY("Texture SizeY", range(1,2048)) = 256//size
		[MaterialToggle] Sharpness ("gaussian blur or Sharpness", Float) = 0
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
			#pragma multi_compile __ SHARPNESS_ON

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
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord; 
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

       		half _TexSizeX;
       		half _TexSizeY;

			fixed4 SampleSpriteTexture (float2 uv)
			{   
#ifdef SHARPNESS_ON
				fixed4 color = frag_boxblur(sharpening_matrix ,_MainTex,uv,half2(_TexSizeX,_TexSizeY));
#else
				fixed4 color = frag_boxblur(gaussian_matrix ,_MainTex,uv,half2(_TexSizeX,_TexSizeY));
#endif

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
