using UnityEngine;
using UnityEditor;

namespace UnityEditor.UI{
	[CustomEditor(typeof(UnityEngine.UI.PageView), true)]
	public class PageViewEditor : ScrollRectEditor {

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			(target as UnityEngine.UI.PageView).movementType = UnityEngine.UI.ScrollRect.MovementType.Unrestricted;
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("pageIndicator"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("pageDamp"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("dragEnableOutSide"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("autoInit"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("isLoop"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("onScrollOver"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("onPageChange"), false);
			serializedObject.ApplyModifiedProperties();
		}
}
}