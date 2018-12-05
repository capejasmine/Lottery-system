using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Particle2DUGUI))]
[CanEditMultipleObjects]
public class Particle2DUGUIEditor : Editor {

	public override void OnInspectorGUI(){
		Particle2DUGUI particle2D = target as Particle2DUGUI;
		EditorGUILayout.Space();

		Color c = GUI.backgroundColor;
		if(particle2D.material==null){
			GUI.backgroundColor =  Color.red;
			EditorGUILayout.TextArea("Error: Material is NULL");
			GUI.backgroundColor =  c;
		} else if(particle2D.mainTexture==null){
			GUI.backgroundColor =  Color.red;
			EditorGUILayout.TextArea("Error: Texture is NULL");
			GUI.backgroundColor =  c;
		}

		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Material"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Color"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("speedScale"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("playOnAwake"), true);
		if(particle2D.playOnAwake){
			EditorGUILayout.PropertyField(serializedObject.FindProperty("delayPlay"), true);
		}
		if(!particle2D.configValues.isLooop){
			EditorGUILayout.PropertyField(serializedObject.FindProperty("autoRemove"), true);
		}else{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("prewarm"), true);
		}
		EditorGUILayout.PropertyField(serializedObject.FindProperty("simulationSpace"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("rectTransAutosize"), true);
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("effectConfig"), true);

		c = GUI.backgroundColor;
		GUI.backgroundColor =  Color.yellow;
		EditorGUILayout.TextArea("Support: Pex Or Plist config File.");

		GUI.backgroundColor =  Color.green;
		if(particle2D.effectConfig!=null){
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Load From Config",GUILayout.Height(24))){
				particle2D.ReadConfig();
				particle2D.ResetParticle();
			}
			EditorGUILayout.EndHorizontal();
		}else{
			GUI.enabled = false;
			GUILayout.Button("Load From Config",GUILayout.Height(24));
			GUI.enabled = true;
		}
		GUI.backgroundColor = c;

		EditorGUILayout.PropertyField(serializedObject.FindProperty("configValues"),true);
		serializedObject.ApplyModifiedProperties();

		c = GUI.backgroundColor;
		GUI.backgroundColor =  Color.green;
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Refresh",GUILayout.Height(32))){
			particle2D.ResetParticle();
		}
		EditorGUILayout.EndHorizontal();
		GUI.backgroundColor = c;

		if(!Application.isPlaying && Selection.activeObject==particle2D.gameObject){
			if(particle2D.configValues!=null && particle2D.Emitter!=null && particle2D.configValues.maxParticles!=particle2D.Emitter.capacity){
				particle2D.ResetParticle();
			}
			EditorUtility.SetDirty (particle2D);
			HandleUtility.Repaint();
		}
	}

	void OnSceneGUI(){
		Particle2DUGUI particle2D = target as Particle2DUGUI;

		Handles.BeginGUI();
		GUI.backgroundColor =  Color.green;
		GUILayout.BeginArea(new Rect(5,5,60,130));
		if (GUILayout.Button("Play",GUILayout.Width(60),GUILayout.Height(30)))
		{
			if(particle2D.Emitter!=null) {
				particle2D.Emitter.Play();
			}else{
				particle2D.ResetParticle();
			}
			particle2D.OnRebuildRequested();
		}
		if (GUILayout.Button("Stop",GUILayout.Width(60),GUILayout.Height(30)))
		{
			if(particle2D.Emitter!=null) {
				if(particle2D.Emitter!=null) {
					particle2D.Emitter.Stop(false);
				}
			}
		}
		if (GUILayout.Button("Clear",GUILayout.Width(60),GUILayout.Height(30)))
		{
			if(particle2D.Emitter!=null) {
				if(particle2D.Emitter!=null) {
					particle2D.Emitter.Stop(true);
					particle2D.OnRebuildRequested();
				}
			}
		}
		GUILayout.EndArea();
		Handles.EndGUI();
	}

	void OnEnable(){
		if(!Application.isPlaying){
			Particle2DUGUI particle2D = target as Particle2DUGUI;
			if(particle2D!=null && Selection.activeGameObject == particle2D.gameObject && particle2D.playOnAwake) {
				if(particle2D.Emitter!=null) {
					particle2D.Emitter.Play();
				}else{
					if(particle2D.configValues==null) particle2D.configValues = new Particle2DConfig();
					if(particle2D.configValues!=null && particle2D.configValues.texture==null){
						//show default texture
						Object[] unityAssets = AssetDatabase.LoadAllAssetsAtPath("Resources/unity_builtin_extra");
						foreach(Object obj in unityAssets){
							if(obj.name.Equals("Default-Particle")){
								particle2D.configValues.texture = obj as Texture;
								break;
							}
						}
					}
					particle2D.ResetParticle();
				}
			}
		}
	}

	void OnDisable(){
		if(!Application.isPlaying){
			Particle2DUGUI particle2D = target as Particle2DUGUI;
			if(particle2D!=null && Selection.activeGameObject != particle2D.gameObject && particle2D.Emitter!=null) {
				if(particle2D.Emitter!=null) {
					particle2D.Emitter.Stop(true);
					particle2D.OnRebuildRequested();
				}
			}
		}
	}

	[MenuItem("Particle2D/Particle2D UGUI")]
	static void CreateParticle2DSystem(){
		GameObject go = new GameObject("Particle2D UGUI");
		Particle2DUGUI ugui = go.AddComponent<Particle2DUGUI>();
		GameObject canvas = GameObject.Find("Canvas");
		if(canvas){
			go.transform.SetParent(canvas.transform);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
		}
		MonoScript ms = MonoScript.FromMonoBehaviour(ugui);
		string filePath = AssetDatabase.GetAssetPath( ms );
		filePath = filePath.Substring(0,filePath.LastIndexOf("Scripts/Particle2DUGUI.cs"));
		filePath += "Materials/UGUI_Additive.mat";
		Material mat = AssetDatabase.LoadAssetAtPath<Material>(filePath);
		ugui.material = mat;
	}
}
