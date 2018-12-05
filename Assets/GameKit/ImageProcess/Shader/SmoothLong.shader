// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Game/ImageProcess/SmoothLong" { 
		Properties{
			_MainTex("Main Texture", 2D) = "Transparent" {}
			_Width("width",Range(0,4096)) = 600 
			_Height("height",Range(0,1024)) = 315
			_BeautyLevel("beauty level",Range(0.3333,1)) = 0.6888
		}

		SubShader{
			Tags{
				"IgnoreProjector" = "True"
				"PreviewType" = "Plane"
			}
				Lighting Off Cull Off ZTest Always ZWrite Off Fog{ Mode Off }


				Pass{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};


				struct v2f {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};


				sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				uniform float _Width;
				uniform float _Height;
				uniform float _Para;
				v2f vert(appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
					return o;
				}

				float3 mix(float3 x,float3 y,float a){
					return x * (1-a) + y*a;
				}

				float hardLight(float color){
					if(color <= 0.5)
					{
						color = color * color * 2.0;
					}
					else
					{
						color = 1.0 - (1.0 - color) * (1.0 - color) * 2.0;
					}
					return color;
				}

				fixed4 frag(v2f i) : COLOR
				{
					
					fixed3 col = tex2D(_MainTex, i.texcoord).rgb ;
					float2 singleStepOffset = float2(2.0/_Width,2.0/_Height);
					float2 blurCoordinates[20];
					blurCoordinates[0] = i.texcoord.xy + singleStepOffset * float2(0.0, -10.0);
					blurCoordinates[1] = i.texcoord.xy + singleStepOffset * float2(0.0, 10.0);
					blurCoordinates[2] = i.texcoord.xy + singleStepOffset * float2(-10.0, 0.0);
					blurCoordinates[3] = i.texcoord.xy + singleStepOffset * float2(10.0, 0.0);
					blurCoordinates[4] = i.texcoord.xy + singleStepOffset * float2(5.0, -8.0);
					blurCoordinates[5] = i.texcoord.xy + singleStepOffset * float2(5.0, 8.0);
					blurCoordinates[6] = i.texcoord.xy + singleStepOffset * float2(-5.0, 8.0);
					blurCoordinates[7] = i.texcoord.xy + singleStepOffset * float2(-5.0, -8.0);
					blurCoordinates[8] = i.texcoord.xy + singleStepOffset * float2(8.0, -5.0);
					blurCoordinates[9] = i.texcoord.xy + singleStepOffset * float2(8.0, 5.0);
					blurCoordinates[10] = i.texcoord.xy + singleStepOffset * float2(-8.0, 5.0);
					blurCoordinates[11] = i.texcoord.xy + singleStepOffset * float2(-8.0, -5.0);
					blurCoordinates[12] = i.texcoord.xy + singleStepOffset * float2(0.0, -6.0);
					blurCoordinates[13] = i.texcoord.xy + singleStepOffset * float2(0.0, 6.0);
					blurCoordinates[14] = i.texcoord.xy + singleStepOffset * float2(6.0, 0.0);
					blurCoordinates[15] = i.texcoord.xy + singleStepOffset * float2(-6.0, 0.0);
					blurCoordinates[16] = i.texcoord.xy + singleStepOffset * float2(-4.0, -4.0);
					blurCoordinates[17] = i.texcoord.xy + singleStepOffset * float2(-4.0, 4.0);
					blurCoordinates[18] = i.texcoord.xy + singleStepOffset * float2(4.0, -4.0);
					blurCoordinates[19] = i.texcoord.xy + singleStepOffset * float2(4.0, 4.0);



					float sampleColor = col.g * 20.0;
					sampleColor += tex2D(_MainTex, blurCoordinates[0]).g;
					sampleColor += tex2D(_MainTex, blurCoordinates[1]).g;
					sampleColor += tex2D(_MainTex, blurCoordinates[2]).g;
					sampleColor += tex2D(_MainTex, blurCoordinates[3]).g;
					sampleColor += tex2D(_MainTex, blurCoordinates[4]).g;
					sampleColor += tex2D(_MainTex, blurCoordinates[5]).g;
					sampleColor += tex2D(_MainTex, blurCoordinates[6]).g;
					sampleColor += tex2D(_MainTex, blurCoordinates[7]).g;
					sampleColor += tex2D(_MainTex, blurCoordinates[8]).g;
					sampleColor += tex2D(_MainTex, blurCoordinates[9]).g;
					sampleColor += tex2D(_MainTex, blurCoordinates[10]).g;
					sampleColor += tex2D(_MainTex, blurCoordinates[11]).g;
					sampleColor += tex2D(_MainTex, blurCoordinates[12]).g * 2.0;
					sampleColor += tex2D(_MainTex, blurCoordinates[13]).g * 2.0;
					sampleColor += tex2D(_MainTex, blurCoordinates[14]).g * 2.0;
					sampleColor += tex2D(_MainTex, blurCoordinates[15]).g * 2.0;
					sampleColor += tex2D(_MainTex, blurCoordinates[16]).g * 2.0;
					sampleColor += tex2D(_MainTex, blurCoordinates[17]).g * 2.0;
					sampleColor += tex2D(_MainTex, blurCoordinates[18]).g * 2.0;
					sampleColor += tex2D(_MainTex, blurCoordinates[19]).g * 2.0;

					sampleColor = sampleColor / 48.0;

					float highPass = col.g - sampleColor + 0.5;

					for(int i = 0; i < 5;i++)
					{
						highPass = hardLight(highPass);
					}

					float3 W = float3(0.299,0.587,0.114);
					float luminance = dot(col, W);


					float alpha = pow(luminance, _Para);

					float3 smoothColor = col + (col-float3(highPass,highPass,highPass))*alpha*0.1;

//						return float4(sampleColor,sampleColor,sampleColor,1);
//						return float4(col.ggg,1);
//						return float4(highPass,highPass,highPass,1);
//						return float4(tex2D(_MainTex, blurCoordinates[0]).ggg,1);
//						return float4(sampleColor,sampleColor,sampleColor,1);
//						return float4(float3(luminance,luminance,luminance),1);
//						return float4(mix(smoothColor.rgb,max(smoothColor,col),alpha), 1.0);
					return float4(smoothColor.rgb * (1 - alpha) + max(smoothColor,col) * alpha, 1.0);
				}



				ENDCG
			}
		}
	
}