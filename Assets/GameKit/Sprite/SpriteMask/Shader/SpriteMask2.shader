// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


//结合SpriteMask.cs使用
Shader "_Game/Mask/Sprite Mask 2"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode",float)=0
		[MaterialToggle] MaskInvert ("Mask Invert", Float) = 0
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

		Cull [_CullMode]
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ MASKINVERT_ON
			#include "UnityCG.cginc"
			#include "MaskCG.cginc"
			
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
				float4 maskUV : TEXCOORD1;
			};

			fixed4 _Color;

			float4 _MaskClip;
			float _Rotation;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;

                float4 muv = GetMaskUV2(IN.vertex,_MaskClip.xy,_MaskClip.zw,_Rotation);
                OUT.maskUV.xy = muv.xy+IN.texcoord;
                OUT.maskUV.zw = muv.zw+IN.texcoord;

				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

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
				c.rgb *= c.a;

				float2 factor = float2(0.0,0.0);
				float2 tempXY = step(IN.maskUV.xy, IN.texcoord);
				factor = max(factor,tempXY);
				float2 tempZW = step(IN.texcoord,IN.maskUV.zw);
				factor = min(factor,tempZW);
				fixed alpha= clamp(min(factor.x,factor.y),0.0,1.0) ;

				#ifdef MASKINVERT_ON
				c *= 1-alpha;
				#else
				c *= alpha;
				#endif

				return c;
			}
		ENDCG
		}
	}
}
