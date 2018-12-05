using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(SpriteDrag))]
[CanEditMultipleObjects]
public class SpriteDragEditor : Editor {

	public override void OnInspectorGUI ()
	{
		SpriteDrag drager = target as SpriteDrag;
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragTarget"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("rayCastCamera"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragRayCastMask"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dropRayCastMask"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("raycastDepth"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragCheckUGUI"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragIgnoreTop"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dropIgnoreBottom"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isDragOriginPoint"), false);
		if(drager.isDragOriginPoint){
			EditorGUILayout.PropertyField(serializedObject.FindProperty("dragOffset"), false);
		}
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragOffsetZ"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragChangeScale"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dragChangeRotate"), false);
	
		EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerPos"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerType"), false);
		if(drager.triggerType== SpriteDrag.TriggerType.Circle){
			EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerRadius"), false);
		}else if(drager.triggerType== SpriteDrag.TriggerType.Range){
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
			if(drager.backEffect!= SpriteDrag.DragBackEffect.None && 
				drager.backEffect!= SpriteDrag.DragBackEffect.Destroy && 
				drager.backEffect!= SpriteDrag.DragBackEffect.Immediately)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("backDuring"), false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("tweenEase"), false);
			}
		}
		serializedObject.ApplyModifiedProperties();
	}
}
