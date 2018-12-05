using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

[CustomEditor(typeof(USprite))]
[CanEditMultipleObjects]
public class USpriteEditor : Editor {

	string[] sortingLayerNames;
	int selectedOption;

	void OnEnable(){
		USprite sp = target as USprite;
		sortingLayerNames = GetSortingLayerNames();
		selectedOption = GetSortingLayerIndex(sp.sortingLayerName);  
	}
	public override void OnInspectorGUI(){
		USprite sp = target as USprite;

		selectedOption = EditorGUILayout.Popup("Sorting Layer", selectedOption, sortingLayerNames);
		if (sortingLayerNames[selectedOption] != sp.sortingLayerName)
		{
			Undo.RecordObject(sp, "Sorting Layer");
			sp.sortingLayerName = sortingLayerNames[selectedOption];
			EditorUtility.SetDirty(sp);
		}
		int newSortingLayerOrder = EditorGUILayout.IntField("Order in Layer", sp.sortingOrder);
		if (newSortingLayerOrder != sp.sortingOrder)
		{
			Undo.RecordObject(sp, "Edit Sorting Order");
			sp.sortingOrder = newSortingLayerOrder;
			EditorUtility.SetDirty(sp);
		}

		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Sprite"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Color"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Material"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Pivot"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_skewX"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_skewY"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_RotatePerspective"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_PerspectiveRange"), true);
		serializedObject.ApplyModifiedProperties();
	}

	public string[] GetSortingLayerNames() {
		System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null, new object[0]);
	}
	public int[] GetSortingLayerUniqueIDs()
	{
		System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
		return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
	}
	int GetSortingLayerIndex(string layerName){
		for(int i = 0; i < sortingLayerNames.Length; ++i){  
			if(sortingLayerNames[i] == layerName) return i;  
		}  
		return 0;  
	}
}
