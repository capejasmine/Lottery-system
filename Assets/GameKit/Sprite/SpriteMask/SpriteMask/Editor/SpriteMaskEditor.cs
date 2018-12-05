using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteMask))]
[CanEditMultipleObjects]
public class SpriteMaskEditor : Editor {

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("maskType"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("updateAlways"), true);
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("maskSize"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("maskMaterials"), true);
		serializedObject.ApplyModifiedProperties();
	}
}
