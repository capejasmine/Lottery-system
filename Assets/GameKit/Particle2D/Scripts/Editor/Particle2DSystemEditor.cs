using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

[CustomEditor(typeof(Particle2DSystem))]
[CanEditMultipleObjects]
public class Particle2DSystemEditor : Editor {

	string[] sortingLayerNames;
	int selectedOption;

	public override void OnInspectorGUI(){
		Particle2DSystem particle2D = target as Particle2DSystem;
		#if UNITY_5_5_OR_NEWER
		EditorUtility.SetSelectedRenderState(particle2D.meshRenderer, EditorSelectedRenderState.Hidden);
		#else
		EditorUtility.SetSelectedWireframeHidden(particle2D.meshRenderer, true);
		#endif
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

		selectedOption = EditorGUILayout.Popup("Sorting Layer", selectedOption, sortingLayerNames);
		if (sortingLayerNames[selectedOption] != particle2D.sortingLayerName)
		{
			Undo.RecordObject(particle2D, "Sorting Layer");
			particle2D.sortingLayerName = sortingLayerNames[selectedOption];
			EditorUtility.SetDirty(particle2D);
		}
		int newSortingLayerOrder = EditorGUILayout.IntField("Order in Layer", particle2D.sortingOrder);
		if (newSortingLayerOrder != particle2D.sortingOrder)
		{
			Undo.RecordObject(particle2D, "Edit Sorting Order");
			particle2D.sortingOrder = newSortingLayerOrder;
			EditorUtility.SetDirty(particle2D);
		}

		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Material"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("color"), true);
		EditorGUILayout.Space();
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
		GUI.backgroundColor = Color.green;
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
		Particle2DSystem particle2D = target as Particle2DSystem;
		Handles.BeginGUI();
		GUI.backgroundColor = Color.green;
		GUILayout.BeginArea(new Rect(5,5,60,130));
		if (GUILayout.Button("Play",GUILayout.Width(60),GUILayout.Height(30)))
		{
			if(particle2D.Emitter!=null) {
				particle2D.Emitter.Play();
			}else{
				particle2D.ResetParticle();
			}
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
				}
			}
		}
		GUILayout.EndArea();
		Handles.EndGUI();
	}


	void OnEnable(){
		Particle2DSystem particle2D = target as Particle2DSystem;
		sortingLayerNames = GetSortingLayerNames();
		selectedOption = GetSortingLayerIndex(particle2D.sortingLayerName);  

		if(!Application.isPlaying){
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
			Particle2DSystem particle2D = target as Particle2DSystem;
			if(particle2D!=null && Selection.activeGameObject != particle2D.gameObject && particle2D.Emitter!=null) {
				if(particle2D.Emitter!=null) {
					particle2D.Emitter.Stop(true);
				}
			}
		}
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

	[MenuItem("Particle2D/Particle2D System")]
	static void CreateParticle2DSystem(){
		GameObject go = new GameObject("Particle2D System");
		go.transform.localScale = Vector3.one*0.02f;
		Particle2DSystem particle  = go.AddComponent<Particle2DSystem>();
		particle.meshFilter.hideFlags= HideFlags.HideInInspector;
		particle.meshRenderer.hideFlags= HideFlags.HideInInspector;
		#if UNITY_5_5_OR_NEWER
		EditorUtility.SetSelectedRenderState(particle.meshRenderer, EditorSelectedRenderState.Hidden);
		#else
		EditorUtility.SetSelectedWireframeHidden(particle.meshRenderer, true);
		#endif
		MonoScript ms = MonoScript.FromMonoBehaviour(particle);
		string filePath = AssetDatabase.GetAssetPath( ms );
		filePath = filePath.Substring(0,filePath.LastIndexOf("Scripts/Particle2DSystem.cs"));
		filePath += "Materials/Additive.mat";
		Material mat = AssetDatabase.LoadAssetAtPath<Material>(filePath);
		particle.material = mat;
	}
}
