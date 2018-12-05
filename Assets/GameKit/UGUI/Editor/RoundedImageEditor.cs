using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityEngine.UI
{
	[CustomEditor(typeof(RoundedImage))]
	public class RoundedImageEditor : Editor {

		private RoundedImage _img = null;

		void OnEnable(){
			_img = target as RoundedImage;
		}

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();
			if(_img.texture!=null && GUILayout.Button("Set Native Size")){
				_img.rectTransform.sizeDelta= new Vector2(_img.texture.width,_img.texture.height);
			}
		}
	}

}