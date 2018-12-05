// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Game/Filter2D/Sprite Water Color"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_NoiseTex ("Noise Texture", 2D) = "white" {}
		_QuatLevel("Quat Level",Range(0,5)) = 5
		_WaterPower ("Water Power", Range(0,100)) = 20
        _TexSizeX("Texture SizeX", range(1,2048)) = 256//size  
        _TexSizeY("Texture SizeY", range(1,2048)) = 256//size
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
			#pragma multi_compile _ OUTLINESOLID_ON
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

			sampler2D _NoiseTex;
       		half _TexSizeX;
       		half _TexSizeY;
			float _QuatLevel ;
			float _WaterPower;

			fixed4 SampleSpriteTexture (float2 uv)
			{         
				fixed4 color = frag_watercolor(_MainTex,uv,_NoiseTex,uv,half2(_TexSizeX,_TexSizeY),_QuatLevel,_WaterPower);

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
