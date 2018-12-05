using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteMask2))]
[CanEditMultipleObjects]
public class SpriteMask2Editor : Editor {

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("updateAlways"), true);
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("maskSize"), true);
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("maskRenderer"), true);
		serializedObject.ApplyModifiedProperties();

		if(GUILayout.Button("Collect Renders")){
			SpriteMask2 sm = target as SpriteMask2;
			sm.maskRenderer = new List<Renderer>(sm.transform.GetComponentsInChildren<Renderer>(true));
			EditorUtility.SetDirty(sm);
			if (!string.IsNullOrEmpty(sm.gameObject.scene.name)){
				UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
			}
		}
	}
}
