// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//author:zhouzhanglin
//预乘Alpha，图片的Advanced设置中Transparent要取消勾选
Shader "_Game/Native2D/Sprite Premultiply Alpha"
{
	Properties
	{
		[HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode",float)=0
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
	
		Lighting off
		Zwrite off
		Cull [_CullMode]
		Blend One OneMinusSrcAlpha

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
				fixed4 color:COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 color:COLOR;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;

			fixed4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color*_Color;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D (_MainTex, i.uv);
				col*=col.a;
				return col*i.color;
			}
			ENDCG
		}
	}
}
