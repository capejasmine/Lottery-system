using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DynamicAsset)),CanEditMultipleObjects]
public class DynamicAssetEditor : Editor {

	private DynamicAsset _da;

	void OnEnable(){
		_da = target as DynamicAsset;
	}

	public override void OnInspectorGUI ()
	{
		if(_da.gameObject.activeInHierarchy==false){
			base.OnInspectorGUI();
			return;
		}

		MonoScript script;
		script = MonoScript.FromMonoBehaviour((DynamicAsset)target);
		script = EditorGUILayout.ObjectField("Script:", script, typeof(MonoScript), false) as MonoScript;

		GUILayout.Space(10f);
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("autoLoad"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("textureReadonly"), false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("cacheAsset"), false);
		if(!_da.cacheAsset){
			EditorGUILayout.PropertyField(serializedObject.FindProperty("autoDisposeAsset"), false);
		}
		EditorGUILayout.PropertyField(serializedObject.FindProperty("_url"), false);
		//路径有变化
		if(!Application.isPlaying && _da.asset!=null && !string.IsNullOrEmpty(_da.url) && !_da.url.Equals(_da.asset.url)){
			_da.frameName="";
			_da.RemoveSprite(true);
		}

		var dragArea = GUILayoutUtility.GetRect (0f, 35f, GUILayout.ExpandWidth (true));  
		GUIContent title = new GUIContent ("Drag Object here from StreamingAssets");  
		GUI.Box (dragArea, title);

		if(_da.url.LastIndexOf(".xml")>0){
//			EditorGUILayout.PropertyField(serializedObject.FindProperty("_frameName"), false);
			//用列表显示
			if(!Application.isPlaying && _da.asset!=null && _da.asset.sprites==null){
				_da.LoadSprite();
			}
			if(_da.asset!=null && _da.asset.sprites!=null){
				int selctedIndex = -1;
				System.Collections.Generic.List<string> animsList =new System.Collections.Generic.List<string>(_da.asset.sprites.Count+1);
				int i=0;
				foreach(string n in _da.asset.sprites.Keys){
					if (n.Equals(_da.frameName)){
						selctedIndex = i;
					}
					animsList.Add(n);
					++i;
				}
				animsList.Insert(0,"<None>");

				int temp = EditorGUILayout.Popup("Sprite Frames", selctedIndex+1, animsList.ToArray())-1;
				if(selctedIndex!=temp){
					if(temp==-1){
						_da.frameName = "";
					}else{
						_da.frameName = animsList[temp+1];
					}
					UnityEditor.EditorUtility.SetDirty(_da);
					if (!Application.isPlaying && !string.IsNullOrEmpty(_da.gameObject.scene.name)){
						UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
					}
				}
			}

		}
		else
		{
			GUILayout.Space(10f);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("replaceMatTexture"), false);
			if(_da.replaceMatTexture){
				EditorGUILayout.PropertyField(serializedObject.FindProperty("material"), false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("nameId"), false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("restoreOnDisable"), false);
				if(_da.restoreOnDisable){
					EditorGUILayout.PropertyField(serializedObject.FindProperty("prevTex"), false);
				}
			}
			else if(_da.material!=null && _da.prevTex!=null)
			{
				_da.material.SetTexture(_da.nameId,_da.prevTex);
			}
		}
		EditorGUILayout.PropertyField(serializedObject.FindProperty("onInited"), false);
		serializedObject.ApplyModifiedProperties();

		GUILayout.Space(10f);
		if(GUILayout.Button("Refresh",GUILayout.Height(20))){
			_da.LoadSprite();
		}

		DropEvent(dragArea);
	}

	void DropEvent(Rect dragArea){
		Event aEvent;  
		aEvent = Event.current;  
		switch (aEvent.type) {  
		case EventType.dragUpdated:  
		case EventType.dragPerform:  
			if (!dragArea.Contains (aEvent.mousePosition)) {  
				break;  
			}  

			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;  
			if (aEvent.type == EventType.DragPerform) {  
				DragAndDrop.AcceptDrag ();  

				for (int i = 0; i < DragAndDrop.objectReferences.Length; ++i) {  
					UnityEngine.Object temp = DragAndDrop.objectReferences [i];  
					if (temp != null) {
						string url = GetObjectPath (temp);
						if(!string.IsNullOrEmpty(url) && !url.Equals(_da.url)){
							_da.url = url;
							_da.LoadSprite();

							if (!Application.isPlaying && !string.IsNullOrEmpty(_da.gameObject.scene.name)){
								UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
							}
						}
						break;  
					}  
				}  
			}
			Event.current.Use ();  
			break;  
		default:  
			break;  
		}  
	}

	string GetObjectPath (Object obj)
	{
		string url = AssetDatabase.GetAssetPath (obj);
		if(url.IndexOf("StreamingAssets")==-1) return "";
		url = url.Replace("Assets/StreamingAssets/", "");
		url = url.Replace(".xml.txt",".xml");
		url = url.Replace(".png.txt",".png");
		url = url.Replace(".jpg.txt",".jpg");
		url = url.Replace(".JPG.txt",".JPG");
		url = url.Replace(".PNG.txt",".PNG");
		url = url.Replace(".JPEG.txt",".JPEG");
		return url;
	}
}