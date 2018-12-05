using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Paint3D))]
public class Paint3DEditor : Editor {

	private Paint3D m_painter;

	void OnEnable(){
		m_painter = target as Paint3D;
	}

//	public override void OnInspectorGUI(){
//
//
//
//	}
}
