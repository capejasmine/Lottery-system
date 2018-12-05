// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

#ifndef MASK2D_CG_INCLUDED
#define MASK2D_CG_INCLUDED

//bl = bottom left coordinate, tr = top right coordinate
// mask layer can rotate
inline float4 GetMaskUV(float4 vertex,float2 bl,float2 tr)
{
	float3 worldPos = mul(unity_ObjectToWorld,vertex);
	float2 l1 = mul(unity_WorldToObject,float4(worldPos-bl,0,0));
	float2 l3 = mul(unity_WorldToObject,float4(worldPos-tr,0,0));

	float4 uv ;
    uv.xy = l1/(l3-l1);
    uv.zw = l3/(l3-l1);
    return uv;
}

// masked spriet can rotate
inline float4 GetMaskUV1(float4 vertex,float2 bl,float2 tr)
{
	float3 worldPos = mul(unity_ObjectToWorld,vertex);
	float2 l1 = worldPos-bl;
	float2 l3 = worldPos-tr;

	float4 uv ;
    uv.xy = l1/(l3-l1);
    uv.zw = l3/(l3-l1);
    return uv;
}

inline float2 RotateUV(float2 uv,float angle)
{
	float rot = angle / 57.32484;
//    uv.xy -=0.5;
    float s = sin ( rot );
    float c = cos ( rot );
    float2x2 rotationMatrix = float2x2( c, -s, s, c);
    uv.xy = mul ( uv.xy, rotationMatrix );
//    uv.xy += 0.5;
    return uv;
}

//bl = bottom left coordinate, tr = top right coordinate
// mask layer can rotate
inline float4 GetMaskUV2(float4 vertex,float2 bl,float2 tr,float angle)
{
	float2 l1 = bl-vertex;
	float2 l3 = tr-vertex;
    l1 = RotateUV(l1,angle);
    l3 = RotateUV(l3,angle);

	float4 uv ;
    uv.xy = l1/(l3-l1);
    uv.zw = l3/(l3-l1);

    return uv;
}

//float CheckInBox (float2 p)
//{
//    bool inside = false;
//
//    for ( int i = 0, j = 3 ; i < 4 ; j = i++ )
//    {
//        if ( (_polygon[i].y > p.y) != (_polygon[j].y > p.y) &&
//             p.x < ( _polygon[j].x - _polygon[i].x ) * ( p.y - _polygon[i].y ) / ( _polygon[j].y - _polygon[i].y ) + _polygon[i].x )
//        {
//            inside = !inside;
//        }
//    }
//    return inside ? 1 : 0 ;
//}

#endif