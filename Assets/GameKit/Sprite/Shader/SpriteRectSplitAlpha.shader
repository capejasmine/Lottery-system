// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//author:zhouzhanglin
//Sprite 显示范围裁切,要注意Sprite的pivot位置
Shader "_Game/Native2D/Sprite Rect Split Alpha"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_AlphaTex ("Alpha Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_ClipRect("Rect" , Vector) = (-1,-1,1,1)
        [MaterialToggle] Invert("Invert",Int) = 0
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

		Cull back
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		colormask RGB

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ INVERT_ON
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			
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
				half2 texcoord1  : TEXCOORD2;
				float4 worldpos : TEXCOORD1;
			};
			
			fixed4 _Color;
			float4 _ClipRect;

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _AlphaTex;
			float4 _AlphaTex_ST;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.worldpos = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
				OUT.texcoord1 = TRANSFORM_TEX(IN.texcoord, _AlphaTex);
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 SampleSpriteTexture (float2 uv,float2 uv1)
			{
				fixed4 color = tex2D (_MainTex, uv);
				color.a = tex2D (_AlphaTex,uv1).r;
				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord,IN.texcoord1) * IN.color;
				c.rgb *= c.a;

				#ifdef INVERT_ON
				c *= 1-UnityGet2DClipping(IN.worldpos.xy, _ClipRect);
				#else
				c *= UnityGet2DClipping(IN.worldpos.xy, _ClipRect);
				#endif

				clip (c.a - 0.001);
				return c;
			}
		ENDCG
		}
	}
}
