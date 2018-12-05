using UnityEngine;

public static class TransformExtension {

	public static void SetPosX(this Transform t, float newX)  
	{  
		t.position = new Vector3(newX, t.position.y, t.position.z);  
	}  

	public static void SetPosY(this Transform t, float newY)  
	{  
		t.position = new Vector3(t.position.x, newY, t.position.z);  
	}  

	public static void SetPosZ(this Transform t, float newZ)  
	{  
		t.position = new Vector3(t.position.x, t.position.y, newZ);  
	}  


	public static void SetLocalPosX(this Transform t, float newX)  
	{  
		t.localPosition = new Vector3(newX, t.localPosition.y, t.localPosition.z);  
	}  

	public static void SetLocalPosY(this Transform t, float newY)  
	{  
		t.localPosition = new Vector3(t.localPosition.x, newY, t.localPosition.z);  
	}  

	public static void SetLocalPosZ(this Transform t, float newZ)  
	{  
		t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, newZ);  
	}







	public static void SetAnchoredPosX(this RectTransform t, float newX)  
	{  
		t.anchoredPosition = new Vector2(newX, t.anchoredPosition.y);  
	}  

	public static void SetAnchoredPosY(this RectTransform t, float newY)  
	{  
		t.anchoredPosition = new Vector2(t.anchoredPosition.x , newY);  
	}

	public static void SetWidth(this RectTransform t, float newX)  
	{  
		t.sizeDelta = new Vector2(newX, t.sizeDelta.y);
	}

	public static void SetHeight(this RectTransform t, float newY)  
	{  
		t.sizeDelta = new Vector2(t.sizeDelta.x, newY);
	}


	public static void SetLocalScaleX(this Transform t, float newX)  
	{  
		t.localScale = new Vector3(newX, t.localScale.y, t.localScale.z);  
	}  

	public static void SetLocalScaleY(this Transform t, float newY)  
	{  
		t.localScale = new Vector3(t.localScale.x, newY, t.localScale.z);  
	}  

	public static void SetLocalScaleZ(this Transform t, float newZ)  
	{  
		t.localScale = new Vector3(t.localScale.x, t.localScale.y, newZ);  
	}
}
