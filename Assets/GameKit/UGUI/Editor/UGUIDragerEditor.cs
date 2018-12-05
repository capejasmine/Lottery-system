using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(UGUIDrag))]
[CanEditMultipleObjects]
public class UGUIDragerEditor : Editor {

	public override void OnInspectorGUI ()
	{
		UGUIDrag drager = target as UGUIDrag;
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragTarget"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("rayCastCamera"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("rayCastMask"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("raycastDepth"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isDragOriginPoint"), false);
		if(drager.isDragOriginPoint){
			EditorGUILayout.PropertyField(serializedObject.FindProperty("dragOffset"), false);
		}
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragOffsetZ"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragChangeScale"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragChangeRotate"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragingParentNode"), false);
		if(!drager.dragingParentNode){
			EditorGUILayout.PropertyField(serializedObject.FindProperty("dragingParent"), false);
		}
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragOnPointerDown"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerPos"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerType"), false);
		if(drager.triggerType== UGUIDrag.TriggerType.Circle){
			EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerRadius"), false);
		}else if(drager.triggerType== UGUIDrag.TriggerType.Range){
			EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerRange"), false);
		}
		EditorGUILayout.PropertyField(serializedObject.FindProperty("sendHoverEvent"), false);
		if(drager.sendHoverEvent){
			EditorGUILayout.PropertyField(serializedObject.FindProperty("onHoverMethodName"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("onHoverOutMethodName"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("onDropMethodName"), false);
		}
		EditorGUILayout.PropertyField(serializedObject.FindProperty("releaseAutoBack"), false);
		if(drager.releaseAutoBack){
			EditorGUILayout.PropertyField(serializedObject.FindProperty("backEffect"), false);
			if(drager.backEffect!= UGUIDrag.DragBackEffect.None && 
				drager.backEffect!= UGUIDrag.DragBackEffect.Destroy && 
				drager.backEffect!= UGUIDrag.DragBackEffect.Immediately)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("backDuring"), false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("tweenEase"), false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("backKeepTop"), false);
			}
		}
		serializedObject.ApplyModifiedProperties();
	}
}
