using UnityEngine;
using System.Collections;

/// <summary>
/// unity2D的锚点.
/// author:zhouzhanglin
/// </summary>
[ExecuteInEditMode]
public class Native2dAnchor : MonoBehaviour {
	
	public enum Position
	{
		CENTER,
		TOP,
		BOTTOM,
		LEFT,
		RIGHT,
		TOP_LEFT,
		TOP_RIGHT,
		BOTTOM_LEFT,
		BOTTOM_RIGHT
	}
	
	public Position position=Position.CENTER;
	public bool updateAlways = false;
	//如果canvas不为空，则使用canvas来适配
	public RectTransform canvas;
	
	IEnumerator Start()
	{
		SetPosition();
		yield return new WaitForEndOfFrame();
		if(!updateAlways)
			SetPosition();
	}
	
	void LateUpdate()
	{
		if(Application.platform== RuntimePlatform.OSXEditor||Application.platform== RuntimePlatform.WindowsEditor){
			if(canvas) SetPositionByCanvas();
			else SetPosition();
		}else if(updateAlways){
			if(canvas) SetPositionByCanvas();
			else SetPosition();
		}
	}

	void SetPositionByCanvas()
	{
		float z = transform.localPosition.z;
		switch (position)
		{
		case Position.CENTER:

			break;
		case Position.TOP:
			transform.position = new Vector3(0f,canvas.sizeDelta.y*0.5f,0f)*canvas.localScale.x;
			break;
		case Position.BOTTOM:
			transform.position = new Vector3(0f,-canvas.sizeDelta.y*0.5f,0f)*canvas.localScale.x;
			break;
		case Position.LEFT:
			transform.position = new Vector3(-canvas.sizeDelta.x*0.5f,0f,0f)*canvas.localScale.x;
			break;
		case Position.RIGHT:
			transform.position = new Vector3(canvas.sizeDelta.x*0.5f,0f,0f)*canvas.localScale.x;
			break;
		case Position.TOP_LEFT:
			transform.position = new Vector3(-canvas.sizeDelta.x*0.5f,canvas.sizeDelta.y*0.5f,0f)*canvas.localScale.x;
			break;
		case Position.TOP_RIGHT:
			transform.position = new Vector3(canvas.sizeDelta.x*0.5f,canvas.sizeDelta.y*0.5f,0f)*canvas.localScale.x;
			break;
		case Position.BOTTOM_LEFT:
			transform.position = new Vector3(-canvas.sizeDelta.x*0.5f,-canvas.sizeDelta.y*0.5f,0f)*canvas.localScale.x;
			break;
		case Position.BOTTOM_RIGHT:
			transform.position = new Vector3(canvas.sizeDelta.x*0.5f,-canvas.sizeDelta.y*0.5f,0f)*canvas.localScale.x;
			break;
		}
		Vector3 v = (Vector3)canvas.position*0.01f + transform.localPosition;
		v.z = z;
		transform.localPosition = v;
	}

	void SetPosition()
	{
		float z = transform.localPosition.z;
		switch (position)
		{
		case Position.CENTER:
			transform.position = Native2dScreenUtil.GetScreenCenter();
			break;
		case Position.TOP:
			transform.position = Native2dScreenUtil.GetScreenTopCenter();
			break;
		case Position.BOTTOM:
			transform.position = Native2dScreenUtil.GetScreenBottomCenter();
			break;
		case Position.LEFT:
			transform.position = Native2dScreenUtil.GetScreenMiddleLeft();
			break;
		case Position.RIGHT:
			transform.position = Native2dScreenUtil.GetScreenMiddleRight();
			break;
		case Position.TOP_LEFT:
			transform.position = Native2dScreenUtil.GetScreenTopLeft();
			break;
		case Position.TOP_RIGHT:
			transform.position = Native2dScreenUtil.GetScreenTopRight();
			break;
		case Position.BOTTOM_LEFT:
			transform.position = Native2dScreenUtil.GetScreenBottomLeft();
			break;
		case Position.BOTTOM_RIGHT:
			transform.position = Native2dScreenUtil.GetScreenBottomRight();
			break;
		}
		Vector3 v = transform.localPosition;
		v.z = z;
		transform.localPosition = v;
	}
}
