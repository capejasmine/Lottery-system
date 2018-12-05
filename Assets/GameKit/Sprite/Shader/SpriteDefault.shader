// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Game/Native2D/Default"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PreAlpha ("Pre multi alpha", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode",float)=0
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc("Src Factor",float)=1
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendDst("Dst Factor",float)=10
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

		Lighting Off
		ZWrite Off
		Cull [_CullMode]
		Blend [_BlendSrc] [_BlendDst]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PREALPHA_ON
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

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				#ifdef PREALPHA_ON
				c *= c.a;
				#else
				c.rgb *= c.a;
				#endif
				return c;
			}
		ENDCG
		}
	}
}
