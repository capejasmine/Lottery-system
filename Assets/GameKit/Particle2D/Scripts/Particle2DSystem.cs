using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode,DisallowMultipleComponent,RequireComponent(typeof(MeshFilter)),RequireComponent(typeof(MeshRenderer))]
public class Particle2DSystem : MonoBehaviour {
	[SerializeField]
	private Material m_Material = null;
	public Material material{
		get{ return m_Material;}
		set{
			m_Material = value;
			meshRenderer.sharedMaterial = value;
			#if UNITY_EDITOR 
			if(!Application.isPlaying) UnityEditor.EditorUtility.SetDirty(meshRenderer);
			#endif
		}
	}

	public Texture mainTexture {
		get { 
			if(configValues!=null) {
				if(configValues.sprite) return configValues.sprite.texture;
				if(configValues.texture) return configValues.texture;
			}
			return material.mainTexture;
		}
	}

	protected MeshFilter m_MeshFilter;
	public MeshFilter meshFilter {
		get {
			if(m_MeshFilter == null) m_MeshFilter = GetComponent<MeshFilter>();
			if(m_MeshFilter == null) m_MeshFilter = gameObject.AddComponent<MeshFilter>();
			return m_MeshFilter;
		}
	}

	protected MeshRenderer m_MeshRenderer;
	public MeshRenderer meshRenderer {
		get {
			if(m_MeshRenderer == null) m_MeshRenderer = GetComponent<MeshRenderer>();
			if(m_MeshRenderer == null) m_MeshRenderer = gameObject.AddComponent<MeshRenderer>();
			return m_MeshRenderer;
		}
	}

	public Color color = Color.white;
	[SerializeField]
	protected string m_SortingLayerName = "Default";
	/// <summary>
	/// Name of the Renderer's sorting layer.
	/// </summary>
	public string sortingLayerName
	{
		get {
			return m_SortingLayerName;
		}
		set {
			m_SortingLayerName = value;
			meshRenderer.sortingLayerName=value;
			#if UNITY_EDITOR 
			if(!Application.isPlaying) UnityEditor.EditorUtility.SetDirty(meshRenderer);
			#endif
		}
	}

	[SerializeField]
	protected int m_SortingOrder = 0;
	/// <summary>
	/// Renderer's order within a sorting layer.
	/// </summary>
	public int sortingOrder
	{
		get {
			return m_SortingOrder;
		}
		set {
			m_SortingOrder = value;
			meshRenderer.sortingOrder=value;
			#if UNITY_EDITOR 
			if(!Application.isPlaying) UnityEditor.EditorUtility.SetDirty(meshRenderer);
			#endif
		}
	}

	[Range(0.1f,2f)]
	public float speedScale = 1f;
	public bool playOnAwake = true;
	public float delayPlay = 0f; //if playOnAwake is true
	public bool prewarm = false;
	public bool autoRemove = false;	//if loop is false
	public Space simulationSpace = Space.World;
	public TextAsset effectConfig;
	public Particle2DConfig configValues;

	[HideInInspector]
	[SerializeField]
	private Particle2D m_Particle2D;
	public Particle2D Emitter{
		get { return m_Particle2D; }
	}

	void OnEnable(){
		meshRenderer.enabled=true;
	}
	void OnDisable(){
		meshRenderer.enabled = false;
	}

	void Start(){
		if(configValues!=null && configValues.texture==null && material!=null && material.mainTexture!=null){
			configValues.texture = material.mainTexture;
		}
		Init();
		if(playOnAwake && delayPlay>0f){
			Invoke("Play",delayPlay);
		}
	}

	void Init(){
		meshRenderer.sortingLayerName = m_SortingLayerName;
		meshRenderer.sortingOrder = m_SortingOrder;

		if(material==null && meshRenderer.sharedMaterial!=null){
			material = meshRenderer.sharedMaterial;
		}
		if(material !=null && mainTexture!=null){
			if(m_Particle2D==null) m_Particle2D = new Particle2D();
			m_Particle2D.simulationSpace = simulationSpace;
			m_Particle2D.material = material;
			m_Particle2D.Init(transform,playOnAwake && delayPlay<=0f);
			m_Particle2D.SetParticle2DConfigVO(configValues,prewarm);

			meshRenderer.sharedMaterial = material;
			meshFilter.mesh = m_Particle2D.mesh;

			MaterialPropertyBlock block = new MaterialPropertyBlock();
			meshRenderer.GetPropertyBlock(block);
			block.SetTexture("_MainTex",mainTexture);
			meshRenderer.SetPropertyBlock(block);
		}
	}

	public void Play(){
		if(m_Particle2D!=null) {
			m_Particle2D.Play();
		}
	}
	public void Stop(bool clear = false){
		if(m_Particle2D!=null) {
			m_Particle2D.Stop(clear);
		}
	}

	#if UNITY_EDITOR
	void Update(){
		if(!Application.isPlaying && meshRenderer){
			meshRenderer.sharedMaterial = material;
		}
		if(configValues!=null && configValues.texture==null){
			//show default texture
			Object[] unityAssets = AssetDatabase.LoadAllAssetsAtPath("Resources/unity_builtin_extra");
			foreach(Object obj in unityAssets){
				if(obj.name.Equals("Default-Particle")){
					configValues.texture = obj as Texture;
					break;
				}
			}
		}

		if(!Application.isPlaying && m_Particle2D!=null && m_Particle2D.isPlaying){
			UpdateParticles();
		}
	}
	#endif

	void LateUpdate(){
		if(Application.isPlaying && m_Particle2D!=null && m_Particle2D.isPlaying){
			UpdateParticles();
			if(autoRemove && !configValues.isLooop && m_Particle2D.isOver){
				Destroy(gameObject);
			}
		}
	}
		
	public void UpdateParticles(){
		if(material!=null && mainTexture!=null && m_Particle2D!=null && m_Particle2D.mesh!=null){
			meshRenderer.enabled = !m_Particle2D.isOver;
			m_Particle2D.simulationSpace = simulationSpace;
			m_Particle2D.material = material;
			m_Particle2D.color = color;
			#if UNITY_EDITOR
			if(!Application.isPlaying)//update Material and texture
			{
				m_Particle2D.UpdateUV();
				MaterialPropertyBlock block = new MaterialPropertyBlock();
				meshRenderer.GetPropertyBlock(block);
				block.SetTexture("_MainTex",mainTexture);
				meshRenderer.SetPropertyBlock(block);
			}
			#endif
			configValues.texture = mainTexture;
			m_Particle2D.AdvanceTime(Time.deltaTime*speedScale);
		}
	}

	void OnDestroy(){
		if(m_Particle2D!=null){
			m_Particle2D.Destroy();
		}
		material = null;
		configValues = null;
		effectConfig = null;
	}

	public void ResetParticle(){
		if(Emitter!=null) Emitter.Stop(true);
		Init();
	}

	#if UNITY_EDITOR
	public void ReadConfig(){
		if(effectConfig!=null){
			Texture t = null;
			if(configValues!=null) t = configValues.texture;
			if(effectConfig.name.ToLower().IndexOf(".pex")>-1){
				configValues = PexConfig.ParsePexConfig(effectConfig.ToString());
			}else if(effectConfig.name.ToLower().IndexOf(".plist")>-1){
				configValues = PlistConfig.ParsePlistConfig(effectConfig.ToString());
			}
			configValues.texture = t;
			if(Emitter!=null) Emitter.Stop(true);
			Init();
			effectConfig = null;
		}
	}
	void OnDrawGizmos(){
		if(Selection.activeTransform==this.transform){
			Gizmos.color = Color.red;
			Matrix4x4 oldMat = Gizmos.matrix;
			if(m_Particle2D!=null && m_Particle2D.config!=null){
				Gizmos.color = new Color(0f,1f,0,0.2f);
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.DrawWireCube(transform.position,new Vector3(
					m_Particle2D.config.emitterXVariance*2f,
					m_Particle2D.config.emitterYVariance*2f
					,0.01f));
			}
			Gizmos.matrix = oldMat;
		}
	}
	#endif
}
