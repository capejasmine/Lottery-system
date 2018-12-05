using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
	[RequireComponent(typeof(PolygonCollider2D))]
	public class UIPolygon : Image
	{

		private PolygonCollider2D _polygon = null;
		private PolygonCollider2D polygon 
		{
			get{
				if(_polygon == null )
					_polygon = GetComponent<PolygonCollider2D>();
				return _polygon;
			}
		}
		public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			if(!raycastTarget || !polygon.enabled) return false;

			if(eventCamera==null) eventCamera=Camera.main;
			return polygon.OverlapPoint( eventCamera.ScreenToWorldPoint(screenPoint));
		}

		#if UNITY_EDITOR
		protected override void Reset()
		{
			base.Reset();
			if(polygon.bounds.size.sqrMagnitude<1){
				float w = (rectTransform.sizeDelta.x *0.5f) + 0.1f;
				float h = (rectTransform.sizeDelta.y*0.5f)  + 0.1f;
				polygon.points = new Vector2[] 
				{
					new Vector2(-w,-h),
					new Vector2(w,-h),
					new Vector2(w,h),
					new Vector2(-w,h)
				};
			}
		}
		#endif
	}

}