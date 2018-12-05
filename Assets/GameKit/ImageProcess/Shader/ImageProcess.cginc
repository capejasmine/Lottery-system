#ifndef IMAGE_PROCESS_INCLUDE
#define IMAGE_PROCESS_INCLUDE
#endif
//#include "HLSLSupport.cginc"
// constant
#define W half3(0.299,0.587,0.114)
#define LUMINANCE_WEIGHTING half3(0.2125, 0.7154, 0.0721)
#define lumaCoeffs half3(0.3,0.59,0.11)
#define saturateMatrix half3x3(1.1402,-0.0598, -0.061,-0.1174,1.0826,-0.1186, -0.0228,-0.0228,1.1772)
// beauty module
float _Width;
float _Height;
float _BeautyLevel;


////////////////////////////
//functions 
///////////////////////////
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

float3 mix(float3 x,float3 y,float a){
	return x * (1-a) + y*a;
}

half4 mix(half4 x,half4 y,float a){
	return x * (1-a) + y*a;
}

half4 gaussianBlur(sampler2D mainTex,float2 itexcoord) { 
	fixed strength = 1.; 
	half4 col = half4(0,0,0,0); 
	float2 sampleStep  = float2(0,0); 

	col += tex2D(mainTex,itexcoord)* 0.25449 ; 
	float width = 1.0/_Width;
	float height = 1.0/_Height;

	sampleStep.x = 1.37754 * width  * strength; 
	sampleStep.y = 1.37754 * height * strength;
	col += tex2D(mainTex,itexcoord+sampleStep) * 0.24797; 
	col += tex2D(mainTex,itexcoord-sampleStep) * 0.24797; 

	sampleStep.x = 3.37754 * width  * strength; 
	sampleStep.y = 3.37754 * height * strength; 
	col += tex2D(mainTex,itexcoord+sampleStep) * 0.09122; 
	col += tex2D(mainTex,itexcoord-sampleStep) * 0.09122; 

	sampleStep.x = 5.37754 * width  * strength; 
	sampleStep.y = 5.37754 * height * strength; 

	col += tex2D(mainTex,itexcoord+sampleStep) * 0.03356; 
	col += tex2D(mainTex,itexcoord-sampleStep) * 0.03356; 

	return col; 
} 
 
half4 overlayBlending(half4 base,half4 overlay){
	// overlay blending 
    half ra; 
	if (base.r < 0.5) { 
		ra = overlay.r * base.r * 2.0; 
	} else {
		ra = 1.0 - ((1.0 - base.r) * (1.0 - overlay.r) * 2.0); 
	} 

	half ga; 
	if (base.g < 0.5) { 
		ga = overlay.g * base.g * 2.0; 
	} else { 
		ga = 1.0 - ((1.0 - base.g) * (1.0 - overlay.g) * 2.0); 
	} 

	half ba; 
	if (base.b < 0.5) {
		ba = overlay.b * base.b * 2.0; 
	} else { 
		ba = 1.0 - ((1.0 - base.b) * (1.0 - overlay.b) * 2.0); 
	} 
	return half4(ra,ga,ba,1.0);
}


half3 rgb2hsv(half3 c) 
{ 
	half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0); 
	half4 p = mix(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
	half4 q = mix(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r)); 
	
	half d = q.x - min(q.w, q.y); 
	half e = 1.0e-10; 
	return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x); 
} 

half3 hsv2rgb(half3 c) 
{ 
	half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0); 
	half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www); 
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y); 
} 

half4 overlay(half4 c1,half4 c2){
	half4 r1 = half4(0.,0.,0.,1.); 

	r1.r = c1.r < 0.5 ? 2.0*c1.r*c2.r : 1.0 - 2.0*(1.0-c1.r)*(1.0-c2.r); 
	r1.g = c1.g < 0.5 ? 2.0*c1.g*c2.g : 1.0 - 2.0*(1.0-c1.g)*(1.0-c2.g); 
	r1.b = c1.b < 0.5 ? 2.0*c1.b*c2.b : 1.0 - 2.0*(1.0-c1.b)*(1.0-c2.b); 

	return r1; 
}

half4 level0c(half4 col, sampler2D tex) { 
    col.r = tex2D(tex, half2(col.r, 0.)).r; 
    col.g = tex2D(tex, half2(col.g, 0.)).r; 
	col.b = tex2D(tex, half2(col.b, 0.)).r; 
	return col; 
} 


half4 normalFunc(half4 c1, half4 c2, half alpha) { 
    return (c2-c1) * alpha + c1; 
}


half NCGray(half4 color){
	half gray = 0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
	return gray;
}


half4 NCTonemapping(sampler2D tex,half4 color){
	half4 mapped;
	mapped.r = tex2D(tex, half2(color.r, 0.0)).r;
    mapped.g = tex2D(tex, half2(color.g, 0.0)).g;
    mapped.b = tex2D(tex, half2(color.b, 0.0)).b;
    mapped.a = color.a;
    
    return mapped;
}

half4 NCColorControl(half4 color,half saturation,half brightness,half contrast){
	float gray = NCGray(color);
    float te = 1.0-saturation;
    color.rgb = half3(saturation,saturation,saturation) * color.rgb + half3(te,te,te) * half3(gray,gray,gray);
    color.r = clamp(color.r, 0.0, 1.0);
    color.g = clamp(color.g, 0.0, 1.0);
    color.b = clamp(color.b, 0.0, 1.0);
    
    color.rgb = half3(contrast,contrast,contrast) * (color.rgb - half3(0.5,0.5,0.5)) + half3(0.5,0.5,0.5);
    color.r = clamp(color.r, 0.0, 1.0);
    color.g = clamp(color.g, 0.0, 1.0);
    color.b = clamp(color.b, 0.0, 1.0);
    
    color.rgb = color.rgb + half3(brightness,brightness,brightness);
    color.r = clamp(color.r, 0.0, 1.0);
    color.g = clamp(color.g, 0.0, 1.0);
    color.b = clamp(color.b, 0.0, 1.0);
    
    return color;
}



 // hue adjust
 half4 NCHueAdjust(half4 color, float hueAdjust)
{
    half3 kRGBToYPrime = half3(0.299, 0.587, 0.114);
    half3 kRGBToI = half3(0.595716, -0.274453, -0.321263);
    half3 kRGBToQ = half3(0.211456, -0.522591, 0.31135);
    
    half3 kYIQToR   = half3(1.0, 0.9563, 0.6210);
    half3 kYIQToG   = half3(1.0, -0.2721, -0.6474);
    half3 kYIQToB   = half3(1.0, -1.1070, 1.7046);
    
    float yPrime = dot(color.rgb, kRGBToYPrime);
    float I = dot(color.rgb, kRGBToI);
    float Q = dot(color.rgb, kRGBToQ);
    
    float hue = atan(Q/ I);
    float chroma  = sqrt (I * I + Q * Q);
    
    hue -= hueAdjust;
    
    Q = chroma * sin (hue);
    I = chroma * cos (hue);
    
    color.r = dot(half3(yPrime, I, Q), kYIQToR);
    color.g = dot(half3(yPrime, I, Q), kYIQToG);
    color.b = dot(half3(yPrime, I, Q), kYIQToB);
    
    return color;
}
 
 // colorMatrix
 half4 NCColorMatrix(half4 color, float red, float green, float blue, float alpha, half4 bias)
{
    color = color * half4(red, green, blue, alpha) + bias;
    
    return color;
}
 
 // multiply blend
 half4 NCMultiplyBlend(half4 overlay, half4 base)
{
    half4 outputColor;
    
    float a = overlay.a + base.a * (1.0 - overlay.a);
    
    //    // normal blend
    //    outputColor.r = (base.r * base.a + overlay.r * overlay.a * (1.0 - base.a))/a;
    //    outputColor.g = (base.g * base.a + overlay.g * overlay.a * (1.0 - base.a))/a;
    //    outputColor.b = (base.b * base.a + overlay.b * overlay.a * (1.0 - base.a))/a;
    
    
    // multiply blend
    outputColor.rgb = ((1.0-base.a) * overlay.rgb * overlay.a + (1.0-overlay.a) * base.rgb * base.a + overlay.a * base.a * overlay.rgb * base.rgb) / a;
    
    
    outputColor.a = a;
    
    return outputColor;
}

////////////////////////////
//end functions 
///////////////////////////


////////////////////////////
//filter effects
///////////////////////////
inline half4 doSaturation(half4 col,float saturationLevel):COLOR
{
	half luminance = dot(col.rgb,LUMINANCE_WEIGHTING);
	fixed3 greyScale = fixed3(luminance,luminance,luminance);
	half4 rcol = half4(mix(greyScale,col.rgb,saturationLevel),col.w);
	return rcol;
}

inline half4 doWhiten(half4 col,sampler2D mainTex,sampler2D curveTex,float2 itexcoord):COLOR
{
	half4 blurColor; 
	fixed strength = -1.0 / 500.0; 

	half xCoordinate = itexcoord.x; 
	half yCoordinate = itexcoord.y;

	half satura = 0.7; 
	// naver skin 
	blurColor = gaussianBlur(mainTex,itexcoord); 

	//saturation 
  	fixed luminance = dot(blurColor.rgb, LUMINANCE_WEIGHTING); 
	fixed3 greyScaleColor = fixed3(luminance,luminance,luminance); 

	blurColor = half4(mix(greyScaleColor, blurColor.rgb, satura), blurColor.w); 
     
	fixed redCurveValue = tex2D(curveTex, half2(col.r, 0.0)).r; 
	fixed greenCurveValue = tex2D(curveTex, half2(col.g, 0.0)).r; 
    fixed blueCurveValue = tex2D(curveTex, half2(col.b, 0.0)).r; 

	redCurveValue = min(1.0, redCurveValue + strength); 
	greenCurveValue = min(1.0, greenCurveValue + strength); 
	blueCurveValue = min(1.0, blueCurveValue + strength); 

    half4 overlay = blurColor;

	half4 base = half4(redCurveValue, greenCurveValue, blueCurveValue, 1.0); 
    // step4 overlay blending 
    return overlayBlending(base,overlay); 
}

inline half4 doSmooth(sampler2D mainTex,float2 itexcoord):COLOR
{
	half4 col = tex2D(mainTex, itexcoord);
	half3 centralColor = tex2D(mainTex, itexcoord).rgb ;

	float2 singleStepOffset = float2(2.0/_Width,2.0/_Height);
	float2 blurCoordinates[20];
	blurCoordinates[0] = itexcoord.xy + singleStepOffset * half2(0.0, -10.0);
	blurCoordinates[1] = itexcoord.xy + singleStepOffset * half2(0.0, 10.0);
	blurCoordinates[2] = itexcoord.xy + singleStepOffset * half2(-10.0, 0.0);
	blurCoordinates[3] = itexcoord.xy + singleStepOffset * half2(10.0, 0.0);
	blurCoordinates[4] = itexcoord.xy + singleStepOffset * half2(5.0, -8.0);
	blurCoordinates[5] = itexcoord.xy + singleStepOffset * half2(5.0, 8.0);
	blurCoordinates[6] = itexcoord.xy + singleStepOffset * half2(-5.0, 8.0);
	blurCoordinates[7] = itexcoord.xy + singleStepOffset * half2(-5.0, -8.0);
	blurCoordinates[8] = itexcoord.xy + singleStepOffset * half2(8.0, -5.0);
	blurCoordinates[9] = itexcoord.xy + singleStepOffset * half2(8.0, 5.0);
	blurCoordinates[10] = itexcoord.xy + singleStepOffset * half2(-8.0, 5.0);
	blurCoordinates[11] = itexcoord.xy + singleStepOffset * half2(-8.0, -5.0);
	blurCoordinates[12] = itexcoord.xy + singleStepOffset * half2(0.0, -6.0);
	blurCoordinates[13] = itexcoord.xy + singleStepOffset * half2(0.0, 6.0);
	blurCoordinates[14] = itexcoord.xy + singleStepOffset * half2(6.0, 0.0);
	blurCoordinates[15] = itexcoord.xy + singleStepOffset * half2(-6.0, 0.0);
	blurCoordinates[16] = itexcoord.xy + singleStepOffset * half2(-4.0, -4.0);
	blurCoordinates[17] = itexcoord.xy + singleStepOffset * half2(-4.0, 4.0);
	blurCoordinates[18] = itexcoord.xy + singleStepOffset * half2(4.0, -4.0);
	blurCoordinates[19] = itexcoord.xy + singleStepOffset * half2(4.0, 4.0);



	half sampleColor = centralColor.g * 20.0;
	sampleColor += tex2D(mainTex, blurCoordinates[0]).g;
	sampleColor += tex2D(mainTex, blurCoordinates[1]).g;
	sampleColor += tex2D(mainTex, blurCoordinates[2]).g;
	sampleColor += tex2D(mainTex, blurCoordinates[3]).g;
	sampleColor += tex2D(mainTex, blurCoordinates[4]).g;
	sampleColor += tex2D(mainTex, blurCoordinates[5]).g;
	sampleColor += tex2D(mainTex, blurCoordinates[6]).g;
	sampleColor += tex2D(mainTex, blurCoordinates[7]).g;
	sampleColor += tex2D(mainTex, blurCoordinates[8]).g;
	sampleColor += tex2D(mainTex, blurCoordinates[9]).g;
	sampleColor += tex2D(mainTex, blurCoordinates[10]).g;
	sampleColor += tex2D(mainTex, blurCoordinates[11]).g;
	sampleColor += tex2D(mainTex, blurCoordinates[12]).g * 2.0;
	sampleColor += tex2D(mainTex, blurCoordinates[13]).g * 2.0;
	sampleColor += tex2D(mainTex, blurCoordinates[14]).g * 2.0;
	sampleColor += tex2D(mainTex, blurCoordinates[15]).g * 2.0;
	sampleColor += tex2D(mainTex, blurCoordinates[16]).g * 2.0;
	sampleColor += tex2D(mainTex, blurCoordinates[17]).g * 2.0;
	sampleColor += tex2D(mainTex, blurCoordinates[18]).g * 2.0;
	sampleColor += tex2D(mainTex, blurCoordinates[19]).g * 2.0;

	sampleColor = sampleColor / 48.0;

	half highPass = centralColor.g - sampleColor + 0.5;

	for(int i = 0; i < 5;i++)
	{
		highPass = hardLight(highPass);
	}

	half luminance = dot(centralColor, W);

	half alpha = pow(luminance, _BeautyLevel);

	half3 smoothColor = centralColor + (centralColor-float3(highPass,highPass,highPass))*alpha*0.1;


//	return half4(tex2D(mainTex, blurCoordinates[0]).ggg,1);
//	return half4(sampleColor,sampleColor,sampleColor,1);
//	return mix(smoothColor.rgb,max(smoothColor,centralColor),alpha);
//	return half4(float3(0,_Width/1024.0,0),1.0);
//	return half4(float3(highPass,highPass,highPass),1);
//	return half4(centralColor.ggg,1.0);

	half4 rcol = half4(mix(smoothColor.rgb,max(smoothColor,centralColor),alpha), col.a);

	return rcol;
}


inline half4 doBeauty(sampler2D mainTex,sampler2D curveTex,float2 itexcoord):COLOR
{
	fixed4 rcol = doSmooth(mainTex, itexcoord);
//	rcol = doWhiten(rcol,mainTex,curveTex,itexcoord) ;  // 还有一些问题需要后面解决(DONE)
//	rcol = doSaturation(rcol,0.85) * 0.9 ;

	return rcol;
}



inline half4 doSunset(half4 inputColor,sampler2D curve,sampler2D grey1Frame,sampler2D grey2Frame,float2 itexcoord):COLOR
{
	fixed4 textureColor; 
	fixed4 textureColorOri; 
	half xCoordinate = itexcoord.x; 
	half yCoordinate = itexcoord.y; 

	float redCurveValue; 
    float greenCurveValue; 
	float blueCurveValue; 

    half4 grey1Color; 
	half4 grey2Color; 

    textureColor = inputColor; 
	grey1Color = tex2D(grey2Frame, itexcoord); 
	grey2Color = tex2D(grey1Frame, itexcoord); 

	// step1 normal blending with original 
	redCurveValue = tex2D(curve, half2(textureColor.r, 0.0)).r; 
	greenCurveValue = tex2D(curve, half2(textureColor.g, 0.0)).g; 
	blueCurveValue = tex2D(curve, half2(textureColor.b, 0.0)).b; 

    textureColorOri = half4(redCurveValue, greenCurveValue, blueCurveValue, 1.0); 
    textureColor = (textureColorOri - textureColor) * grey1Color.r + textureColor; 

    //DONE:curve texture problem.
	redCurveValue = tex2D(curve, half2(textureColor.r, 0.0)).a; 
	greenCurveValue = tex2D(curve, half2(textureColor.g, 0.0)).a; 
    blueCurveValue = tex2D(curve, half2(textureColor.b, 0.0)).a; 
	//textureColor = half4(redCurveValue, greenCurveValue, blueCurveValue, 1.0); 

    // step3 60% opacity  ExclusionBlending 
	textureColor = half4(redCurveValue, greenCurveValue, blueCurveValue, 1.0); 
    half4 textureColor2 = half4(0.08627, 0.03529, 0.15294, 1.0); 
	textureColor2 = textureColor + textureColor2 - (2.0 * textureColor2 * textureColor); 

    textureColor = (textureColor2 - textureColor) * 0.6784 + textureColor; 


	half4 overlay = half4(0.6431, 0.5882, 0.5803, 1.0); 
	half4 base = textureColor;
 	// overlay blending 
	textureColor = overlayBlending(base,overlay); 
	base = (textureColor - base) + base;

    // again overlay blending 
    overlay = half4(0.0, 0.0, 0.0, 1.0);
	 
    textureColor = overlayBlending(base,overlay); 
	textureColor = (textureColor - base) * (grey2Color * 0.549) + base; 

	return half4(textureColor.r, textureColor.g, textureColor.b, 1.0); 	
}


inline half4 doSunset(sampler2D mainTex,sampler2D curve,sampler2D grey1Frame,sampler2D grey2Frame,float2 itexcoord):COLOR{
	half4 textureColor = tex2D(mainTex,itexcoord);
	return doSunset(textureColor,curve,grey1Frame,grey2Frame,itexcoord);
}


inline half4 doWhiteCat(half4 inputColor,sampler2D curveTex,float2 itexcoord){
	half4 textureColor; 
	half4 textureColorOri;
	float xCoordinate = itexcoord.x;
	float yCoordinate = itexcoord.y; 

	float redCurveValue;
	float greenCurveValue;
	float blueCurveValue; 

	textureColor = inputColor; 

	// step1 20% opacity  ExclusionBlending 
    half4 textureColor2 = textureColor; 
	textureColor2 = textureColor + textureColor2 - (2.0 * textureColor2 * textureColor); 

	textureColor = (textureColor2 - textureColor) * 0.2 + textureColor; 

    // step2 curve 
    redCurveValue = tex2D(curveTex, float2(textureColor.r, 0.0)).r; 
	greenCurveValue = tex2D(curveTex, float2(textureColor.g, 0.0)).g; 
	blueCurveValue = tex2D(curveTex, float2(textureColor.b, 0.0)).b; 

    redCurveValue = tex2D(curveTex, float2(redCurveValue, 1.0)).r; 
	greenCurveValue = tex2D(curveTex, float2(greenCurveValue, 1.0)).r;
	blueCurveValue = tex2D(curveTex, float2(blueCurveValue, 1.0)).r; 

	redCurveValue = tex2D(curveTex, float2(redCurveValue, 1.0)).g; 
	greenCurveValue = tex2D(curveTex, float2(greenCurveValue, 1.0)).g; 
	blueCurveValue = tex2D(curveTex, float2(blueCurveValue, 1.0)).g; 
//	return half4(redCurveValue, greenCurveValue, blueCurveValue,1);

	float3 tColor = float3(redCurveValue, greenCurveValue, blueCurveValue); 
	tColor = rgb2hsv(tColor); 

	tColor.g = tColor.g * 0.765; 

	tColor = hsv2rgb(tColor); 
    tColor = clamp(tColor, 0.0, 1.0); 

    half4 base = half4(tColor, 1.0); 
	half4 overlay = half4(0.62, 0.6, 0.498, 1.0); 
	// step6 overlay blending 
    
	textureColor = overlayBlending(base,overlay);
	textureColor = (textureColor - base) * 0.1 + base; 

	return half4(textureColor.rgb, 1.0); 
}

inline half4 doWhiteCat(sampler2D mainTex,sampler2D curveTex,float2 itexcoord){
	half4 textureColor = tex2D(mainTex,itexcoord);
	return doWhiteCat(textureColor,curveTex,itexcoord);
}



inline half4 doLatte(half4 inputColor,sampler2D curve,float2 itexcoord){
	fixed4 textureColor;
	fixed4 textureColorOri;
	float xCoordinate = itexcoord.x;
	float yCoordinate = itexcoord.y; 

	float redCurveValue; 
	float greenCurveValue; 
	float blueCurveValue;
	
	textureColor = inputColor; 
	half4 base = inputColor; 
	half4 overlay = half4(0.792, 0.58, 0.372, 1.0); 
	
	// step1 overlay blending 
	 
	textureColor = overlayBlending(base,overlay); 
	textureColor = (textureColor - base) * 0.3 + base; 
	
	redCurveValue = tex2D(curve, float2(textureColor.r, 0.0)).r; 
	greenCurveValue = tex2D(curve, float2(textureColor.g, 0.0)).g; 
	blueCurveValue = tex2D(curve, float2(textureColor.b, 0.0)).b; 
	
	redCurveValue = tex2D(curve, float2(redCurveValue, 1.0)).g; 
	greenCurveValue = tex2D(curve, float2(greenCurveValue, 1.0)).g; 
	blueCurveValue = tex2D(curve, float2(blueCurveValue, 1.0)).g; 
	
	
	float3 tColor = float3(redCurveValue, greenCurveValue, blueCurveValue); 
	tColor = rgb2hsv(tColor); 
	
	tColor.g = tColor.g * 0.6; 
	
	float dStrength = 1.0; 
	float dSatStrength = 0.2; 
	
	float dGap = 0.0; 
	
	if( tColor.r >= 0.0 && tColor.r < 0.417) 
	{ 
		tColor.g = tColor.g + (tColor.g * dSatStrength); 
    } 
	else if( tColor.r > 0.958 && tColor.r <= 1.0) 
	{ 
		tColor.g = tColor.g + (tColor.g * dSatStrength); 
    } 
	else if( tColor.r >= 0.875 && tColor.r <= 0.958) 
	{ 
		dGap = abs(tColor.r - 0.875); 
		dStrength = (dGap / 0.0833); 
		
		tColor.g = tColor.g + (tColor.g * dSatStrength * dStrength); 
    } 
	else if( tColor.r >= 0.0417 && tColor.r <= 0.125) 
	{ 
		dGap = abs(tColor.r - 0.125);
		dStrength = (dGap / 0.0833); 
		
		tColor.g = tColor.g + (tColor.g * dSatStrength * dStrength); 
	} 
	
	
	tColor = hsv2rgb(tColor); 
	tColor = clamp(tColor, 0.0, 1.0); 
	
	redCurveValue = tex2D(curve, half2(tColor.r, 1.0)).r; 
	greenCurveValue = tex2D(curve, half2(tColor.g, 1.0)).r; 
	blueCurveValue = tex2D(curve, half2(tColor.b, 1.0)).r; 

	base = half4(redCurveValue, greenCurveValue, blueCurveValue, 1.0); 
	overlay = half4(0.792, 0.494, 0.372, 1.0); 

	// step5 overlay blending 

	textureColor = overlayBlending(base,overlay); 
	textureColor = (textureColor - base) * 0.15 + base; 

	return half4(textureColor.r, textureColor.g, textureColor.b, 1.0) * 0.8; 
}

inline half4 doLatte(sampler2D mainTex,sampler2D curve,float2 itexcoord){
	half4 col = tex2D(mainTex, itexcoord);
	return doLatte(col,curve,itexcoord);
}


inline half4 doToaster2(half4 intputColor,
						 sampler2D metal,
						 sampler2D softLight,
						 sampler2D curve,
						 sampler2D overlaymap,
						 sampler2D colorShift,float2 itexcoord){
	half3 texel;
    half2 lookup;
    float2 blue;
    float2 green;
    float2 red;
    half4 tmpvar_1;
    tmpvar_1 = intputColor;
    texel = tmpvar_1.xyz;
    half4 tmpvar_2;
    tmpvar_2 = tex2D (metal, itexcoord);
    float2 tmpvar_3;
    tmpvar_3.x = tmpvar_2.x;
    tmpvar_3.y = tmpvar_1.x;
    texel.x = tex2D (softLight, tmpvar_3).x;
    float2 tmpvar_4;
    tmpvar_4.x = tmpvar_2.y;
    tmpvar_4.y = tmpvar_1.y;
    texel.y = tex2D (softLight, tmpvar_4).y;
    float2 tmpvar_5;
    tmpvar_5.x = tmpvar_2.z;
    tmpvar_5.y = tmpvar_1.z;
    texel.z = tex2D (softLight, tmpvar_5).z;
    red.x = texel.x;
    red.y = 0.16666;
    green.x = texel.y;
    green.y = 0.5;
    blue.x = texel.z;
    blue.y = 0.833333;
    texel.x = tex2D (curve, red).x;
    texel.y = tex2D (curve, green).y;
    texel.z = tex2D (curve, blue).z;
    float2 tmpvar_6;
    tmpvar_6 = ((2.0 * itexcoord) - 1.0);
    float2 tmpvar_7;
    tmpvar_7.x = dot (tmpvar_6, tmpvar_6);
    tmpvar_7.y = texel.x;
    lookup = tmpvar_7;
    texel.x = tex2D (overlaymap, tmpvar_7).x;
    lookup.y = texel.y;
    texel.y = tex2D (overlaymap, lookup).y;
    lookup.y = texel.z;
    texel.z = tex2D (overlaymap, lookup).z;
    red.x = texel.x;
    green.x = texel.y;
    blue.x = texel.z;
    texel.x = tex2D (colorShift, red).x;
    texel.y = tex2D (colorShift, green).y;
    texel.z = tex2D (colorShift, blue).z;
    half4 tmpvar_8;
    tmpvar_8.w = 1.0;
    tmpvar_8.xyz = texel;
    return tmpvar_8;
}


inline half4 doToaster2(sampler2D mainTex,
						 sampler2D metal,
						 sampler2D softLight,
						 sampler2D curve,
						 sampler2D overlaymap,
						 sampler2D colorShift,float2 itexcoord){
	
	half4 col = tex2D(mainTex, itexcoord);
	return doToaster2(col,metal,softLight,curve,overlaymap,colorShift,itexcoord);
}


inline half4 doSketch(half4 inputColor,sampler2D mainTex,float2 itexcoord){
	float threshold = 0.0;
	//pic1
	half4 oralColor = inputColor;
	
	//pic2
	float3 maxValue = float3(0.,0.,0.);

	float width = 1.0/_Width;
	float height = 1.0/_Height;
	float2 singleStepOffset = (width,height);
	
	for(int i = -2; i<=2; i++)
	{
		for(int j = -2; j<=2; j++)
		{
			half4 tempColor = tex2D(mainTex, itexcoord + singleStepOffset * float2(i,j));
			maxValue.r = max(maxValue.r,tempColor.r);
			maxValue.g = max(maxValue.g,tempColor.g);
			maxValue.b = max(maxValue.b,tempColor.b);
			threshold += dot(tempColor.rgb, W);
		}
	}
	//pic3
	float gray1 = dot(oralColor.rgb, W);
	
	//pic4
	float gray2 = dot(maxValue, W);
	
	//pic5
	float contour = gray1 / gray2;

	threshold = threshold / 25.;
	float alpha = max(1.0,gray1>threshold?1.0:(gray1/threshold));
	
	float result = contour * alpha + (1.0-alpha)*gray1;
	
	return half4(result,result,result, oralColor.w);
}

inline half4 doSketch(sampler2D mainTex,float2 itexcoord){
	half4 col = tex2D(mainTex,itexcoord);
	return doSketch(col,mainTex,itexcoord);
}

inline half4 doValencia(half4 inputColor,sampler2D mainTex,sampler2D map,sampler2D gradMap,float2 itexcoord){
	half4 originColor = inputColor;
	float3 texel = inputColor.rgb;

	texel = half3(
	          tex2D(map, float2(texel.r, .1666666)).r,
	          tex2D(map, float2(texel.g, .5)).g,
	          tex2D(map, float2(texel.b, .8333333)).b
	          );
	          //TODO:
//	texel = mul(saturateMatrix , texel);
//	float luma = dot(lumaCoeffs, texel);
//	texel = half3(
//	          tex2D(gradMap, float2(luma, texel.r)).r,
//	          tex2D(gradMap, float2(luma, texel.g)).g,
//	          tex2D(gradMap, float2(luma, texel.b)).b);
	texel.rgb = mix(originColor.rgb, texel, 0.15);
	return half4(texel, 1.0);
}

inline half4 doValencia(sampler2D mainTex,sampler2D map,sampler2D gradMap,float2 itexcoord){
	half4 col = tex2D(mainTex,itexcoord);

	return doValencia(col,mainTex,map,gradMap,itexcoord);
}




inline half4 doSakura(half4 inputColor,sampler2D mainTex,sampler2D curveTex,float2 itexcoord){
	fixed4 c0 = inputColor;
	fixed4 c1 = gaussianBlur(mainTex,itexcoord); 
	fixed4 c2 = overlay(c0, level0c(c1, curveTex)); 
	fixed4 c3 = normalFunc(c0,c2,0.15);

	return c3;
}

inline half4 doSakura(sampler2D mainTex,sampler2D curveTex,float2 itexcoord){
	half4 col = tex2D(mainTex,itexcoord);
	return doSakura(col,mainTex,curveTex,itexcoord);
}


inline half4 doRomance(half4 inputColor,sampler2D curveTex,float2 itexcoord){
	fixed4 textureColor;
	fixed4 textureColorRes; 
	fixed4 textureColorOri; 
	half4 grey1Color; 
	half4 layerColor; 
	half satVal = 115.0 / 100.0; 

    float xCoordinate = itexcoord.x;
	float yCoordinate = itexcoord.y; 
	
	float redCurveValue; 
	float greenCurveValue; 
	float blueCurveValue; 

	textureColor = inputColor; 
    textureColorRes = textureColor; 
	textureColorOri = textureColor; 

	// step1. screen blending 
	textureColor = 1.0 - ((1.0 - textureColorOri) * (1.0 - textureColorOri)); 
	textureColor = (textureColor - textureColorOri) + textureColorOri; 

	// step2. curve 
	redCurveValue = tex2D(curveTex, float2(textureColor.r, 0.0)).r; 
	greenCurveValue = tex2D(curveTex, float2(textureColor.g, 0.0)).g; 
    blueCurveValue = tex2D(curveTex, float2(textureColor.b, 0.0)).b; 

	// step3. saturation 
	float G = (redCurveValue + greenCurveValue + blueCurveValue); 
	G = G / 3.0; 

    redCurveValue = ((1.0 - satVal) * G + satVal * redCurveValue); 
	greenCurveValue = ((1.0 - satVal) * G + satVal * greenCurveValue); 
	blueCurveValue = ((1.0 - satVal) * G + satVal * blueCurveValue); 

	textureColor = half4(redCurveValue, greenCurveValue, blueCurveValue, 1.0); 

    return half4(textureColor.r, textureColor.g, textureColor.b, 1.0) * 0.8; 
}

inline half4 doRomance(sampler2D mainTex,sampler2D curveTex,float2 itexcoord){
	half4 col = tex2D(mainTex,itexcoord);
	return doRomance(col,curveTex,itexcoord);
}


inline half4 doWarm(half4 inputColor,sampler2D curveTex,sampler2D greyFrame,sampler2D layerImage,float2 itexcoord){
	fixed4 textureColor = inputColor;
	half4 greyColor;
	half4 layerColor;
	
	float xCoordinate = itexcoord.x;
	float yCoordinate = itexcoord.y;
	
	float redCurveValue;
	float greenCurveValue; 
	float blueCurveValue;
	
	greyColor = tex2D(greyFrame, half2(xCoordinate, yCoordinate));
	layerColor = tex2D(layerImage, half2(xCoordinate, yCoordinate));

	// step1 curve
	redCurveValue = tex2D(curveTex, half2(textureColor.r, 0.0)).r; 
	greenCurveValue = tex2D(curveTex, half2(textureColor.g, 0.0)).g;
	blueCurveValue = tex2D(curveTex, half2(textureColor.b, 0.0)).b; 

    // step2 curve with mask 
	textureColor = half4(redCurveValue, greenCurveValue, blueCurveValue, 1.0);

	redCurveValue = tex2D(curveTex, half2(textureColor.r, 0.0)).a;
	greenCurveValue = tex2D(curveTex, half2(textureColor.g, 0.0)).a; 
    blueCurveValue = tex2D(curveTex, half2(textureColor.b, 0.0)).a; 

	fixed4 textureColor2 = half4(redCurveValue, greenCurveValue, blueCurveValue, 1.0); 

	// step3 screen with 60% 
	fixed4 base = half4(mix(textureColor.rgb, textureColor2.rgb, 1.0 - greyColor.r), textureColor.a); 
	fixed4 overlayer = half4(layerColor.r, layerColor.g, layerColor.b, 1.0);

    // screen blending 
	textureColor = 1.0 - ((1.0 - base) * (1.0 - overlayer));
	textureColor = (textureColor - base) * 0.6 + base;
	
	redCurveValue = tex2D(curveTex, half2(textureColor.r, 1.0)).r; 
	greenCurveValue = tex2D(curveTex, half2(textureColor.g, 1.0)).g;
	blueCurveValue = tex2D(curveTex, half2(textureColor.b, 1.0)).b; 
	textureColor = half4(redCurveValue, greenCurveValue, blueCurveValue, 1.0); 

	return half4(textureColor.r, textureColor.g, textureColor.b, 1.0);
}



inline half4 doWarm(sampler2D mainTex,sampler2D curveTex,sampler2D greyFrame,sampler2D layerImage,float2 itexcoord){
	 half4 inputColor = tex2D(mainTex,itexcoord);
	 return doWarm(inputColor,curveTex,greyFrame,layerImage,itexcoord);
}

inline half4 doTender(half4 inputColor,sampler2D curveTex,sampler2D grey1Frame,float2 itexcoord){
	half4 textureColor = inputColor;
	half4 textureColorRes;
	half4 grey1Color;
	half satVal = 65.0 / 100.0; 
	half mask1R = 29.0 / 255.0; 
	half mask1G = 43.0 / 255.0; 
	half mask1B = 95.0 / 255.0;
	
	float xCoordinate = itexcoord.x;
	float yCoordinate = itexcoord.y;
	
	float redCurveValue;
	float greenCurveValue; 
	float blueCurveValue; 

	textureColorRes = textureColor;

	grey1Color = tex2D(grey1Frame, half2(xCoordinate, yCoordinate)); 

	// step1. saturation
    float G = (textureColor.r + textureColor.g + textureColor.b); 
	G = G / 3.0; 

	redCurveValue = ((1.0 - satVal) * G + satVal * textureColor.r);
	greenCurveValue = ((1.0 - satVal) * G + satVal * textureColor.g); 
	blueCurveValue = ((1.0 - satVal) * G + satVal * textureColor.b); 

	// step2 curve 
    redCurveValue = tex2D(curveTex, half2(textureColor.r, 0.0)).r;
	greenCurveValue = tex2D(curveTex, half2(textureColor.g, 0.0)).g;
	blueCurveValue = tex2D(curveTex, half2(textureColor.b, 0.0)).b;

	// step3 30% opacity  ExclusionBlending
	textureColor = half4(redCurveValue, greenCurveValue, blueCurveValue, 1.0);
	half4 textureColor2 = half4(mask1R, mask1G, mask1B, 1.0);
    textureColor2 = textureColor + textureColor2 - (2.0 * textureColor2 * textureColor); 

	textureColor = (textureColor2 - textureColor) * 0.3 + textureColor; 

	half4 overlay = half4(0, 0, 0, 1.0); 
	half4 base = half4(textureColor.r, textureColor.g, textureColor.b, 1.0); 

	// step4 overlay blending 
	textureColor = overlayBlending(base,overlay); 

	base = (textureColor - base) * (grey1Color.r/2.0) + base; 

	return half4(base.r, base.g, base.b, 1.0);
}


inline half4 doTender(sampler2D mainTex,sampler2D curveTex,sampler2D grey1Frame,float2 itexcoord){
	half4 inputColor = tex2D(mainTex,itexcoord);
	 return doTender(inputColor,curveTex,grey1Frame,itexcoord);
}


inline half4 doAmaro(half4 inputColor,sampler2D blowoutTex,sampler2D overlay,sampler2D map,float2 itexcoord){
	float strength = 0.5; //TODO:

     half4 originColor = inputColor;
     half4 texel = inputColor;
     half3 bbTexel = tex2D(blowoutTex, itexcoord).rgb;
     
     texel.r = tex2D(overlay, half2(bbTexel.r, texel.r)).r;
     texel.g = tex2D(overlay, half2(bbTexel.g, texel.g)).g;
     texel.b = tex2D(overlay, half2(bbTexel.b, texel.b)).b;
     
     half4 mapped;
     mapped.r = tex2D(map, half2(texel.r, .16666)).r;
     mapped.g = tex2D(map, half2(texel.g, .5)).g;
     mapped.b = tex2D(map, half2(texel.b, .83333)).b;
     mapped.a = 1.0;
     
     mapped = mix(originColor, mapped, strength);

     return mapped;
}


inline half4 doAmaro(sampler2D mainTex,sampler2D blowoutTex,sampler2D overlay,sampler2D map,float2 itexcoord){
	half4 inputColor = tex2D(mainTex,itexcoord);
	return doAmaro(inputColor,blowoutTex,overlay,map,itexcoord);
}

inline half4 doPixar(half4 inputColor,sampler2D toneMap,float2 itexcoord){
	half4 originColor = inputColor;
    half4 color = inputColor;
    float strength = 0.5; //TODO:
    color.a = 1.0;
    
    // tone mapping
    color.r = tex2D(toneMap, half2(color.r, 0.0)).r;
    color.g = tex2D(toneMap, half2(color.g, 0.0)).g;
    color.b = tex2D(toneMap, half2(color.b, 0.0)).b;
    
    // color control
    color = NCColorControl(color, 1.0, 0.08, 1.0);
    
    // hue adjust
    color = NCHueAdjust(color, 0.0556);
    
    // color matrix
    color = NCColorMatrix(color, 1.0, 1.0, 1.0, 1.0, half4(0.02, 0.02, 0.06, 0));
    
    color = mix(originColor, color, strength);

    return color;
}

inline half4 doPixar(sampler2D mainTex,sampler2D toneMap,float2 itexcoord){
    half4 col = tex2D(mainTex,itexcoord);
    return doPixar(col,toneMap,itexcoord);
}