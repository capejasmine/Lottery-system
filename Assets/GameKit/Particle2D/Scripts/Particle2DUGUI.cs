using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode, RequireComponent(typeof(CanvasRenderer), typeof(RectTransform)), DisallowMultipleComponent]
public class Particle2DUGUI : MaskableGraphic {

	[Range(0.1f,2f)]
	public float speedScale = 1f;
	public bool playOnAwake = true;
	public float delayPlay = 0f; //if playOnAwake is true
	public bool prewarm = false;
	public bool autoRemove = false;
	public Space simulationSpace = Space.World;
	public bool rectTransAutosize = true;

	public TextAsset effectConfig;
	public Particle2DConfig configValues;

	[HideInInspector]
	[SerializeField]
	private Particle2D m_Particle2D;

	public Particle2D Emitter{
		get { return m_Particle2D; }
	}

	protected override void Start(){
		base.Start();
		this.raycastTarget = false;
		if(configValues!=null && configValues.texture==null && material!=null && material.mainTexture!=null){
			configValues.texture = material.mainTexture;
		}
		Init();
		if(playOnAwake && delayPlay>0f){
			Invoke("Play",delayPlay);
		}
	}

	private void Init(){
		if(material !=null && mainTexture!=null){
			if(m_Particle2D==null) m_Particle2D = new Particle2D();
			m_Particle2D.simulationSpace = simulationSpace;
			m_Particle2D.material = material;
			m_Particle2D.Init(transform,playOnAwake && delayPlay<=0f);
			m_Particle2D.SetParticle2DConfigVO(configValues,prewarm);
			canvasRenderer.SetMaterial(material,mainTexture);
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
			if(clear) canvasRenderer.SetMesh(m_Particle2D.mesh);
		}
	}

	public override void Rebuild (CanvasUpdate update) {
		base.Rebuild(update);
		if (canvasRenderer.cull) return;
		if (update == CanvasUpdate.PreRender){
			UpdateParticles();
			if(m_Particle2D==null || !m_Particle2D.isPlaying){
				canvasRenderer.SetMesh(null);
			}
		}
	}

	public override Texture mainTexture {
		get { 
			if(configValues!=null) {
				if(configValues.sprite) return configValues.sprite.texture;
				if(configValues.texture) return configValues.texture;
			}
			return material.mainTexture;
		}
	}

	protected override void OnPopulateMesh (VertexHelper vh)
	{
		vh.Clear();
	}

	#if UNITY_EDITOR
	void Update(){
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
				if(Application.isPlaying) Destroy(gameObject);
			}
		}
	}

	public void UpdateParticles(){
		if(material!=null && mainTexture!=null && m_Particle2D!=null &&  m_Particle2D.mesh!=null){
			m_Particle2D.simulationSpace = simulationSpace;
			m_Particle2D.material = material;
			#if UNITY_EDITOR
			if(!Application.isPlaying)//update Material and texture
			{
				m_Particle2D.UpdateUV();
				canvasRenderer.SetMaterial(material,mainTexture);
			}
			#endif
			configValues.texture = mainTexture;
			m_Particle2D.color = color;
			m_Particle2D.AdvanceTime(Time.deltaTime*speedScale);
			if(m_Particle2D.isOver){
				canvasRenderer.SetMesh(null);
			}else{
				canvasRenderer.SetMesh(m_Particle2D.mesh);
			}
		}
	}

	protected override void OnDestroy(){
		if(m_Particle2D!=null){
			m_Particle2D.Destroy();
			m_Particle2D = null;
		}
		material = null;
		configValues = null;
		effectConfig = null;

		base.OnDestroy();
	}

	public void ResetParticle(){
		#if UNITY_EDITOR
		base.Reset();
		#endif
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
			if(rectTransAutosize && m_Particle2D!=null){
				rectTransform.sizeDelta=new Vector2(m_Particle2D.config.emitterXVariance*2f,
					m_Particle2D.config.emitterYVariance*2f);
			}

			Gizmos.color = Color.red;
			Matrix4x4 oldMat = Gizmos.matrix;
			if(m_Particle2D!=null && m_Particle2D.config!=null){
				Gizmos.color = new Color(0f,1f,0,0.2f);
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.DrawWireCube(Vector3.zero,new Vector3(
					m_Particle2D.config.emitterXVariance*2f,
					m_Particle2D.config.emitterYVariance*2f
					,0.01f));
			}
			Gizmos.matrix = oldMat;
		}
	}
	#endif
}
