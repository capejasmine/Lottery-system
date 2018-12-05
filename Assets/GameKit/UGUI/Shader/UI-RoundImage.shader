//对单张图片有影响
Shader "_Game/UGUI/Rounded Image"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
      
        _ColorMask("Color Mask", Float) = 15

        _RoundedRadius("Rounded Radius", Range(0, 256)) = 64

		[Toggle(CIRCLE_OR_ROUNDED_MASK)] _CircleOrRounded ("Circle Or Rounded Mask", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode",float)=0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull [_CullMode]
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass
        {
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ CIRCLE_OR_ROUNDED_MASK

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

            struct appdata_t
            {
				float4 vertex   :POSITION;
				float4 color    :COLOR;
				float2 texcoord :TEXCOORD0;
            };

            struct v2f
            {
				float4 vertex   :SV_POSITION;
				fixed4 color :COLOR;
				half2 texcoord  :TEXCOORD0;
				float4 worldPosition :TEXCOORD1;
            };

            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            float _RoundedRadius;

            float4 _MainTex_TexelSize;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = IN.texcoord;

                OUT.color = IN.color * _Color;
                return OUT;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);


                #ifdef CIRCLE_OR_ROUNDED_MASK

                	float r = _RoundedRadius*0.01;
                	r = clamp(r,0.0001,1);
                	half hole = min(distance(IN.texcoord, half2(0.5,0.5)) / r, 1);
             		color.a *= 1-pow(hole, 100);

                #else

                float r = _RoundedRadius;

                float width = _MainTex_TexelSize.z;
                float height = _MainTex_TexelSize.w;

                float x = IN.texcoord.x * width;
                float y = IN.texcoord.y * height;

                //左下角
                if (x < r && y < r)
                {
                    if ((x - r) * (x - r) + (y - r) * (y - r) > r * r)
                        color.a = 0;
                }

                //左上角
                if (x < r && y > (height - r))
                {
                    if ((x - r) * (x - r) + (y - (height - r)) * (y - (height - r)) > r * r)
                        color.a = 0;
                }

                //右下角
                if (x > (width - r) && y < r)
                {
                    if ((x - (width - r)) * (x - (width - r)) + (y - r) * (y - r) > r * r)
                        color.a = 0;
                }

                //右上角
                if (x > (width - r) && y > (height - r))
                {
                    if ((x - (width - r)) * (x - (width - r)) + (y - (height - r)) * (y - (height - r)) > r * r)
                        color.a = 0;
                }
                #endif

                return color;
            }
            ENDCG
        }
    }
}