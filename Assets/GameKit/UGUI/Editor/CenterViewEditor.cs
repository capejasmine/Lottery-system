using UnityEngine;
using UnityEditor;

namespace UnityEditor.UI{
	[CustomEditor(typeof(UnityEngine.UI.CenterView), true)]
	public class CenterViewEditor : ScrollRectEditor {

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();
			(target as UnityEngine.UI.CenterView).movementType = UnityEngine.UI.ScrollRect.MovementType.Unrestricted;
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("pageIndicator"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("pageDamp"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("minScale"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxScale"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("clickItemToCenter"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("autoInit"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("onScrollOver"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("onPageChange"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("onSelect"), false);
			serializedObject.ApplyModifiedProperties();
		}
	}
}