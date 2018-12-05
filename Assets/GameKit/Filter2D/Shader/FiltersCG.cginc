//
// create by zhouzhanglin
//
#ifndef UNITY_CG_FILTERS
#define UNITY_CG_FILTERS
// #include "UnityShaderVariables.cginc"

//outLine =================
//soft edge 
fixed4 frag_outline1(sampler2D mainTex,float2 uv, float outline,fixed4 outlineColor){
	if(outline==0) return tex2D(mainTex,uv);
	fixed4 tempColor = tex2D(mainTex,uv+float2(outline,0.0)) + tex2D(mainTex, uv-float2(outline,0.0));
	tempColor = tempColor + tex2D(mainTex, uv+float2(0.0,outline)) + tex2D(mainTex, uv-float2(0.0,outline));
	fixed4 alphaColor = fixed4(outlineColor.rgb,tempColor.a);
	fixed4 mainColor = alphaColor * outlineColor;
	fixed4 col = tex2D(mainTex, uv);
	col = lerp(mainColor,col,col.a);
    return col;
}
//solid edge
fixed4 frag_outline2(sampler2D mainTex,float2 uv, float outline,fixed4 outlineColor){
	if(outline==0) return tex2D(mainTex,uv);
	fixed4 tempColor = tex2D(mainTex,uv+float2(outline,0.0)) + tex2D(mainTex, uv-float2(outline,0.0));
	tempColor = tempColor + tex2D(mainTex, uv+float2(0.0,outline)) + tex2D(mainTex, uv-float2(0.0,outline));
	fixed4 alphaColor = fixed4(outlineColor.rgb,tempColor.a);
	fixed4 mainColor = alphaColor * outlineColor;
	fixed4 col = tex2D(mainTex, uv);
	if(col.a > 0.95){
       mainColor = col;
    }
    return mainColor;
}


// glow ==============
// horizon glow
fixed4 frag_glow_h(sampler2D mainTex , float2 uv ,fixed4 glowColor, float blurAmount){
	if(blurAmount==0) return tex2D(mainTex,uv);
	fixed4 sum = fixed4(0,0,0,0);
	sum += tex2D(mainTex, float2(uv.x - 5.0 * blurAmount, uv.y)) * 0.025;
	sum += tex2D(mainTex, float2(uv.x - 4.0 * blurAmount, uv.y)) * 0.05;
	sum += tex2D(mainTex, float2(uv.x - 3.0 * blurAmount, uv.y)) * 0.09;
	sum += tex2D(mainTex, float2(uv.x - 2.0 * blurAmount, uv.y)) * 0.12;
	sum += tex2D(mainTex, float2(uv.x - blurAmount, uv.y)) * 0.15;
	sum += tex2D(mainTex, float2(uv.x, uv.y)) * 0.16;
	sum += tex2D(mainTex, float2(uv.x + blurAmount, uv.y)) * 0.15;
	sum += tex2D(mainTex, float2(uv.x + 2.0 * blurAmount, uv.y)) * 0.12;
	sum += tex2D(mainTex, float2(uv.x + 3.0 * blurAmount, uv.y)) * 0.09;
	sum += tex2D(mainTex, float2(uv.x + 4.0 * blurAmount, uv.y)) * 0.05;
	sum += tex2D(mainTex, float2(uv.x + 5.0 * blurAmount, uv.y)) * 0.025;
	sum.rgb = glowColor.rgb*sum.a*glowColor.a;
	return sum;
}
//vertical glow
fixed4 frag_glow_v(sampler2D mainTex , float2 uv ,fixed4 glowColor, float blurAmount){
	if(blurAmount==0) return tex2D(mainTex,uv);
	fixed4 sum = fixed4(0,0,0,0);
    sum += tex2D(mainTex, float2(uv.x, uv.y - 5.0 * blurAmount)) * 0.025;
    sum += tex2D(mainTex, float2(uv.x, uv.y - 4.0 * blurAmount)) * 0.05;
    sum += tex2D(mainTex, float2(uv.x, uv.y - 3.0 * blurAmount)) * 0.09;
    sum += tex2D(mainTex, float2(uv.x, uv.y - 2.0 * blurAmount)) * 0.12;
    sum += tex2D(mainTex, float2(uv.x, uv.y - blurAmount)) * 0.15;
    sum += tex2D(mainTex, float2(uv.x, uv.y)) * 0.16;
    sum += tex2D(mainTex, float2(uv.x, uv.y + blurAmount)) * 0.15;
    sum += tex2D(mainTex, float2(uv.x, uv.y + 2.0 * blurAmount)) * 0.12;
    sum += tex2D(mainTex, float2(uv.x, uv.y + 3.0 * blurAmount)) * 0.09;
    sum += tex2D(mainTex, float2(uv.x, uv.y + 4.0 * blurAmount)) * 0.05;
    sum += tex2D(mainTex, float2(uv.x, uv.y + 5.0 * blurAmount)) * 0.025;
  	sum.rgb = glowColor.rgb*sum.a*glowColor.a;
    return sum;
}


//blur ============================================
fixed4 frag_blur_h(sampler2D mainTex , float2 uv, float blurAmount){
	if(blurAmount==0) return tex2D(mainTex,uv);
	fixed4 sum = fixed4(0,0,0,0);
	sum += tex2D(mainTex, float2(uv.x - 5.0 * blurAmount, uv.y)) * 0.025;
	sum += tex2D(mainTex, float2(uv.x - 4.0 * blurAmount, uv.y)) * 0.05;
	sum += tex2D(mainTex, float2(uv.x - 3.0 * blurAmount, uv.y)) * 0.09;
	sum += tex2D(mainTex, float2(uv.x - 2.0 * blurAmount, uv.y)) * 0.12;
	sum += tex2D(mainTex, float2(uv.x - blurAmount, uv.y)) * 0.15;
	sum += tex2D(mainTex, float2(uv.x, uv.y)) * 0.16;
	sum += tex2D(mainTex, float2(uv.x + blurAmount, uv.y)) * 0.15;
	sum += tex2D(mainTex, float2(uv.x + 2.0 * blurAmount, uv.y)) * 0.12;
	sum += tex2D(mainTex, float2(uv.x + 3.0 * blurAmount, uv.y)) * 0.09;
	sum += tex2D(mainTex, float2(uv.x + 4.0 * blurAmount, uv.y)) * 0.05;
	sum += tex2D(mainTex, float2(uv.x + 5.0 * blurAmount, uv.y)) * 0.025;
	return sum;
}
//vertical blur
fixed4 frag_blur_v(sampler2D mainTex , float2 uv , float blurAmount){
	if(blurAmount==0) return tex2D(mainTex,uv);
	fixed4 sum = fixed4(0,0,0,0);
    sum += tex2D(mainTex, float2(uv.x, uv.y - 5.0 * blurAmount)) * 0.025;
    sum += tex2D(mainTex, float2(uv.x, uv.y - 4.0 * blurAmount)) * 0.05;
    sum += tex2D(mainTex, float2(uv.x, uv.y - 3.0 * blurAmount)) * 0.09;
    sum += tex2D(mainTex, float2(uv.x, uv.y - 2.0 * blurAmount)) * 0.12;
    sum += tex2D(mainTex, float2(uv.x, uv.y - blurAmount)) * 0.15;
    sum += tex2D(mainTex, float2(uv.x, uv.y)) * 0.16;
    sum += tex2D(mainTex, float2(uv.x, uv.y + blurAmount)) * 0.15;
    sum += tex2D(mainTex, float2(uv.x, uv.y + 2.0 * blurAmount)) * 0.12;
    sum += tex2D(mainTex, float2(uv.x, uv.y + 3.0 * blurAmount)) * 0.09;
    sum += tex2D(mainTex, float2(uv.x, uv.y + 4.0 * blurAmount)) * 0.05;
    sum += tex2D(mainTex, float2(uv.x, uv.y + 5.0 * blurAmount)) * 0.025;
    return sum;
}


// gray============
inline fixed3 frag_gray(fixed3 color,fixed progress){
	fixed gray = dot(color.rgb, fixed3(0.299, 0.587, 0.114));
	color.rgb = lerp(color.rgb,fixed3(gray, gray, gray),progress) ;
	return color;
}


// curve=============
fixed4 curve1(sampler2D mainTex,float2 uv,half2 texSize){
   float2  upLeftUV = float2(uv.x - 1.0/texSize.x , uv.y - 1.0/texSize.y);
   fixed4  bkColor = fixed4(0.5 , 0.5 , 0.5 , 1.0);
   fixed4  curColor    =  tex2D( mainTex, uv );
   fixed4  upLeftColor =  tex2D( mainTex, upLeftUV );
   //相减得到颜色的差
   fixed4  delColor = curColor - upLeftColor;
   //需要把这个颜色的差设置
   float  h = 0.3 * delColor.x + 0.59 * delColor.y + 0.11* delColor.z;
   fixed4 outColor =  fixed4(h,h,h,0.0)+ bkColor;
   outColor.a = curColor.a;
   return outColor;
}
fixed4 curve2(sampler2D mainTex,float2 uv,half2 texSize){
	//浮雕就是对图像上的一个像素和它右下的那个像素的色差的一种处理  
    fixed4 curColor = tex2D (mainTex, uv);//当前点的颜色  
    fixed3 mc11 = tex2D (mainTex, uv+fixed2(1/texSize.x,1/texSize.y)).rgb;//当前点右下角（偏移了（1,1）个单位）的点的颜色，  
    //由于CG函数tex2DSize函数（获取图片长宽的像素数）在unity中不能用，我也不知道用什么函数来替代它，就弄了个外部变量_Size方便调节  

    float3 diffs = abs( curColor - mc11);//diffs为亮点颜色差  
    float max0 = diffs.r>diffs.g?diffs.r:diffs.g;  
    max0 = max0>diffs.b?max0:diffs.b;//求出色差中rgb的最大值设为色差数  
    float gray = clamp(max0+0.4 , 0, 1);//灰度值其实就是这个色差数  
    float4 c = 0;  
    c = float4(gray.xxx,1);//最终颜色  
    c.a = curColor.a;
    return c; 
}





//mosaic=================
fixed4 mosaic1(sampler2D mainTex,float2 uv,half2 texSize,half2 mosaicSize){
   //得到当前纹理坐标相对图像大小整数值。
   float2  intXY = float2(uv.x * texSize.x , uv.y * texSize.y);
   //根据马赛克块大小进行取整。
   float2  XYMosaic   = float2(int(intXY.x/mosaicSize.x) * mosaicSize.x,
                               int(intXY.y/mosaicSize.y) * mosaicSize.y );
   //把整数坐标转换回纹理采样坐标
   float2  UVMosaic   = float2(XYMosaic.x/texSize.x , XYMosaic.y/texSize.y);
   return tex2D( mainTex, UVMosaic );
}

fixed4 mosaic2(sampler2D mainTex,float2 uv,half2 texSize,half2 mosaicSize){
   //得到当前纹理坐标相对图像大小整数值。
   float2  intXY = float2(uv.x * texSize.x , uv.y * texSize.y);
   //马赛克中心不再是左上角，而是中心
   float2  XYMosaic   = float2(int(intXY.x/mosaicSize.x) * mosaicSize.x,
   								int(intXY.y/mosaicSize.y) * mosaicSize.y ) + 0.5 *mosaicSize;
   //求出采样点到马赛克中心的距离
   float2  delXY = XYMosaic - intXY;
   float   delL  = length(delXY);
   float2  UVMosaic   = float2(XYMosaic.x/texSize.x , XYMosaic.y/texSize.y);
   float4  finalColor;
   //判断是不是处于马赛克圆中。
   if(delL< 0.5 * mosaicSize.x)
       finalColor = tex2D( mainTex, UVMosaic );
   else
       finalColor = tex2D( mainTex, uv );
   return finalColor;
}


   
//HSV================

fixed3 simple_hsv(fixed3 RGB, half3 shift) // shift is h , s , v
{
     fixed3 RESULT = fixed3(RGB);
     float VSU = shift.z*shift.y*cos(shift.x*3.14159265/180);
       float VSW = shift.z*shift.y*sin(shift.x*3.14159265/180);
         
       RESULT.x = (.299*shift.z+.701*VSU+.168*VSW)*RGB.x
           + (.587*shift.z-.587*VSU+.330*VSW)*RGB.y
           + (.114*shift.z-.114*VSU-.497*VSW)*RGB.z;
         
       RESULT.y = (.299*shift.z-.299*VSU-.328*VSW)*RGB.x
           + (.587*shift.z+.413*VSU+.035*VSW)*RGB.y
           + (.114*shift.z-.114*VSU+.292*VSW)*RGB.z;
         
       RESULT.z = (.299*shift.z-.3*VSU+1.25*VSW)*RGB.x
           + (.587*shift.z-.588*VSU-1.05*VSW)*RGB.y
           + (.114*shift.z+.886*VSU-.203*VSW)*RGB.z;
         
     return (RESULT);
}



//HSV to RGB
float3 HSVConvertToRGB(float3 hsv){
   float R,G,B;
   //float3 rgb;
   if( hsv.y == 0 )
   {
       R=G=B=hsv.z;
   }
   else
   {
       hsv.x = hsv.x/60.0; 
       int i = (int)hsv.x;
       float f = hsv.x - (float)i;
       float a = hsv.z * ( 1 - hsv.y );
       float b = hsv.z * ( 1 - hsv.y * f );
       float c = hsv.z * ( 1 - hsv.y * (1 - f ) );
       if(i==0){
      	  R = hsv.z; G = c; B = a;
       }else if(i==1){
      	  R = b; G = hsv.z; B = a; 
       }else if(i==2){
      	  R = a; G = hsv.z; B = c; 
       }else if(i==3){
      	  R = a; G = b; B = hsv.z; 
       }else if(i==4){
      	  R = c; G = a; B = hsv.z; 
       }else{
          R = hsv.z; G = a; B = b; 
       }
   }
   return float3(R,G,B);
 }


 //RGB to HSV
float3 RGBConvertToHSV(float3 rgb){
    float R = rgb.x,G = rgb.y,B = rgb.z;
    float3 hsv;
    float max1=max(R,max(G,B));
    float min1=min(R,min(G,B));
    if (R == max1) 
    {
        hsv.x = (G-B)/(max1-min1);
    }
    if (G == max1) 
    {
        hsv.x = 2 + (B-R)/(max1-min1);
    }
    if (B == max1) 
    {
        hsv.x = 4 + (R-G)/(max1-min1);
    }
    hsv.x = hsv.x * 60.0;   
    if (hsv.x < 0) 
        hsv.x = hsv.x + 360;
    hsv.z=max1;
    hsv.y=(max1-min1)/max1;
    return hsv;
}


/*
** Hue, saturation, luminance
*/

float3 RGBToHSL(float3 color)
{
	float3 hsl; // init to 0 to avoid warnings ? (and reverse if + remove first part)
	
	float fmin = min(min(color.r, color.g), color.b);    //Min. value of RGB
	float fmax = max(max(color.r, color.g), color.b);    //Max. value of RGB
	float delta = fmax - fmin;             //Delta RGB value

	hsl.z = (fmax + fmin) / 2.0; // Luminance

	if (delta == 0.0)		//This is a gray, no chroma...
	{
		hsl.x = 0.0;	// Hue
		hsl.y = 0.0;	// Saturation
	}
	else                                    //Chromatic data...
	{
		if (hsl.z < 0.5)
			hsl.y = delta / (fmax + fmin); // Saturation
		else
			hsl.y = delta / (2.0 - fmax - fmin); // Saturation
		
		float deltaR = (((fmax - color.r) / 6.0) + (delta / 2.0)) / delta;
		float deltaG = (((fmax - color.g) / 6.0) + (delta / 2.0)) / delta;
		float deltaB = (((fmax - color.b) / 6.0) + (delta / 2.0)) / delta;

		if (color.r == fmax )
			hsl.x = deltaB - deltaG; // Hue
		else if (color.g == fmax)
			hsl.x = (1.0 / 3.0) + deltaR - deltaB; // Hue
		else if (color.b == fmax)
			hsl.x = (2.0 / 3.0) + deltaG - deltaR; // Hue

		if (hsl.x < 0.0)
			hsl.x += 1.0; // Hue
		else if (hsl.x > 1.0)
			hsl.x -= 1.0; // Hue
	}

	return hsl;
}

float HueToRGB(float f1, float f2, float hue)
{
	if (hue < 0.0)
		hue += 1.0;
	else if (hue > 1.0)
		hue -= 1.0;
	float res;
	if ((6.0 * hue) < 1.0)
		res = f1 + (f2 - f1) * 6.0 * hue;
	else if ((2.0 * hue) < 1.0)
		res = f2;
	else if ((3.0 * hue) < 2.0)
		res = f1 + (f2 - f1) * ((2.0 / 3.0) - hue) * 6.0;
	else
		res = f1;
	return res;
}

float3 HSLToRGB(float3 hsl)
{
	float3 rgb;
	
	if (hsl.y == 0.0)
		rgb = float3(hsl.z,hsl.z,hsl.z); // Luminance
	else
	{
		float f2;
		
		if (hsl.z < 0.5)
			f2 = hsl.z * (1.0 + hsl.y);
		else
			f2 = (hsl.z + hsl.y) - (hsl.y * hsl.z);
			
		float f1 = 2.0 * hsl.z - f2;
		
		rgb.r = HueToRGB(f1, f2, hsl.x + (1.0/3.0));
		rgb.g = HueToRGB(f1, f2, hsl.x);
		rgb.b= HueToRGB(f1, f2, hsl.x - (1.0/3.0));
	}
	
	return rgb;
}


//Radial blur ===================
static const float samples[10] = 
{
   -0.08,
   -0.05,
   -0.03,
   -0.02,
   -0.01,
   0.01,
   0.02,
   0.03,
   0.05,
   0.08
};

fixed4 frag_radialblur(sampler2D mainTex,float2 uv , float dist , float strength){
	//0.5,0.5屏幕中心
    float2 dir = float2(0.5, 0.5) - uv;//从采样中心到uv的方向向量
    float _dist = length(dir);  
    dir = normalize(dir); 
    float4 color = tex2D(mainTex, uv);  

    float4 sum = color;
    for (int i = 0; i < 10; ++i)  
    {                   
       sum += tex2D(mainTex, uv + dir * samples[i] * dist);    
    }  
    //求均值
    sum /= 11.0f;  

   
    //越离采样中心近的地方，越不模糊
    float t = saturate(_dist * strength);  

    //插值
    return lerp(color, sum, t);
}

//water color======================
inline float4 quant(float4 cl , float n)
{
	//该函数对颜色的四个分量进行量化
    cl.x = int(cl.x * 255 / n) * n /255;
    cl.y = int(cl.y * 255 / n) * n /255;
    cl.z = int(cl.z * 255 / n) * n /255;
    return cl;
}
fixed4 frag_watercolor(sampler2D mainTex,float2 uv,sampler2D noiseTex,float2 uv1,half2 texSize,float quatLevel,float waterPower){
	//取得随机数，对纹理坐标进行扰动，形成扩散效果
    float4 noiseColor = waterPower * tex2D(noiseTex , uv1);
    float2 newUV  = float2(uv.x + noiseColor.x / texSize.x , uv1.y + noiseColor.y / texSize.y);
    float4 fColor = tex2D(mainTex , newUV);
    //量化图像的颜色值，形成色块
    return quant(fColor , 255/pow(2, quatLevel) );
}



//Dip========================
//box blur
static const float3x3 blur_matrix = float3x3 (1/9.0 ,1/9.0,1/9.0 ,
                                    1/9.0 ,1/9.0,1/9.0 ,
                                    1/9.0 ,1/9.0,1/9.0 );

//高斯模糊
static const float3x3 gaussian_matrix = float3x3 (1/16.0 ,1/8.0,1/16.0 ,
                                    1/8.0 ,1/4.0,1/9.0 ,
                                    1/16.0 ,1/8.0,1/16.0 );

//锐化
static const float3x3 sharpening_matrix = float3x3 (-1,-1,-1,
                                    -1 ,9.0,-1 , 
                                    -1,-1,-1 );

float4 box_filter(float3x3 _filter , sampler2D _image, float2 _xy, float2 texSize)
{
    //最终的输出颜色
    float4 final_color = float4(0.0,0.0,0.0,0.0);
   
    float2 _xy_new = _xy+float2(-1.0 , -1.0);
    float2 _uv_new = float2(_xy_new.x/texSize.x , _xy_new.y/texSize.y);
    final_color += tex2D( _image, _uv_new ) * _filter[0][0];

    _xy_new = _xy+float2(0 , -1.0);
    _uv_new = float2(_xy_new.x/texSize.x , _xy_new.y/texSize.y);
    final_color += tex2D( _image, _uv_new ) * _filter[0][1];

    _xy_new = _xy+float2(1 , -1.0);
    _uv_new = float2(_xy_new.x/texSize.x , _xy_new.y/texSize.y);
    final_color += tex2D( _image, _uv_new ) * _filter[0][2];

    _xy_new = _xy+float2(0 , -1.0);
    _uv_new = float2(_xy_new.x/texSize.x , _xy_new.y/texSize.y);
    final_color += tex2D( _image, _uv_new ) * _filter[1][0];

    _xy_new = _xy+float2(0 , 0);
    _uv_new = float2(_xy_new.x/texSize.x , _xy_new.y/texSize.y);
    final_color += tex2D( _image, _uv_new ) * _filter[1][1];

    _xy_new = _xy+float2(1 , 0);
    _uv_new = float2(_xy_new.x/texSize.x , _xy_new.y/texSize.y);
    final_color += tex2D( _image, _uv_new ) * _filter[1][2];

    _xy_new = _xy+float2(1 , -1);
    _uv_new = float2(_xy_new.x/texSize.x , _xy_new.y/texSize.y);
    final_color += tex2D( _image, _uv_new ) * _filter[2][0];

    _xy_new = _xy+float2(0 , 1);
    _uv_new = float2(_xy_new.x/texSize.x , _xy_new.y/texSize.y);
    final_color += tex2D( _image, _uv_new ) * _filter[2][1];

    _xy_new = _xy+float2(1 , 1);
    _uv_new = float2(_xy_new.x/texSize.x , _xy_new.y/texSize.y);
    final_color += tex2D( _image, _uv_new ) * _filter[2][2];

    return final_color;
}

float4 frag_boxblur(float3x3 smooth_fil ,sampler2D mainTex,float2 uv,float2 texSize){
	float2  intXY = float2(uv.x * texSize.x , uv.y * texSize.y);
	return box_filter(smooth_fil , mainTex , intXY, texSize);
}


//==========photoshop============
//multiply
inline fixed3 frag_multiply(fixed3 baseColor,fixed3 blendColor,fixed opacity){
 	// Perform a multiply Blend mode  
    fixed3 blendedMultiply = baseColor * blendColor;  
    // Adjust amount of Blend Mode with a lerp  
    return lerp(baseColor, blendedMultiply,  opacity);
}
//Screen
inline fixed3 frag_screen(fixed3 baseColor,fixed3 blendColor,fixed opacity){
 	// Perform a multiply Blend mode  
    fixed3 blendedAdd = baseColor + blendColor;  
    // Adjust amount of Blend Mode with a lerp  
    fixed3 blendedScreen = 1.0 - ((1.0 - baseColor) * (1.0 - blendColor));  
    return lerp(baseColor, blendedScreen,  opacity);
}
//overlay
inline fixed overlay(fixed baseColor,fixed blendColor) {  
    if (baseColor < 0.5) {  
        return (2.0 * baseColor * blendColor);  
    } else {  
        return (1.0 - 2.0 * (1.0 - baseColor) * (1.0 - blendColor));  
    }  
}
inline fixed3 frag_overlay(fixed3 baseColor,fixed3 blendColor,fixed opacity) { 
	fixed3 col = blendColor ;
	col.r = overlay(baseColor.r,blendColor.r);
	col.g = overlay(baseColor.g,blendColor.g);
	col.b = overlay(baseColor.b,blendColor.b);
	col = lerp(baseColor, col,  opacity);
	return col;
}
//color
float3 frag_color(fixed4 baseColor, fixed4 blendColor,fixed opacity)
{
	fixed3 blendHSL = RGBToHSL(blendColor);
	fixed3 c = HSLToRGB(float3(blendHSL.r, blendHSL.g, RGBToHSL(baseColor).b));
	c = lerp(baseColor, c, opacity);
	c.rgb = lerp(baseColor.rgb,c.rgb,blendColor.a);
	return c;
}
#endif