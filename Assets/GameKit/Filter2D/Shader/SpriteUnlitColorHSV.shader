// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Game/Filter2D/Sprite Color HSV"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        //Hue的值范围为0-359. 其他两个为0-1 ,这里我们设置到3，因为乘以3后 都不一定能到超过.
        _Hue ("Hue", Range(0,359)) = 0
        _Saturation ("Saturation", Range(0,3.0)) = 1.0
        _Value ("Value", Range(0,3.0)) = 1.0
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
				float2 texcoord  : TEXCOORD0;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord; 
				OUT.color = IN.color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;
       		half _Hue;
	        half _Saturation;
	        half _Value;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D(_MainTex,uv);
				float3 colorHSV=float3(1,1,1);   
	            colorHSV.xyz = RGBConvertToHSV(color.rgb);  //转换为HSV
	            colorHSV.x += _Hue; //调整偏移Hue值
	            colorHSV.x = colorHSV.x%360;    //超过360的值从0开始

	            colorHSV.y *= _Saturation;  //调整饱和度
	            colorHSV.z *= _Value;                           

	            color.rgb = HSVConvertToRGB(colorHSV.xyz);   //将调整后的HSV，转换为RGB颜色


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
				clip (c.a - 0.001);
				return c;
			}
		ENDCG
		}
	}
}
