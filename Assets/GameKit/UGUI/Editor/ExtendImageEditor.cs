using UnityEngine;
using UnityEditor;

namespace UnityEditor.UI{
	[CustomEditor(typeof(UnityEngine.UI.ExtendImage), true)]
	public class ExtendImageEditor : ImageEditor {

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Extend"), true);
			serializedObject.ApplyModifiedProperties();
		}
	}
}