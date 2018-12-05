using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using System.Xml;
using System.IO;
#endif

/// <summary>
/// 用于动态显示StreamingAssets下面的图片
/// </summary>
[ExecuteInEditMode]
public class DynamicAsset : MonoBehaviour {

	#region 静态方法

	/// <summary>
	/// 加载资源并初始化
	/// </summary>
	/// <param name="dynamicAssets">Dynamic Assets.</param>
	/// <param name="onInited">On inited.</param>
	/// <param name="onProgress">On progress.</param>
	public static void Init(DynamicAsset[] dynamicAssets , System.Action onInited, System.Action<float> onProgress = null){
		List<ResManager.Asset> groups = new List<ResManager.Asset>();
		Dictionary<string ,bool> urls = new Dictionary<string, bool>();
		foreach(DynamicAsset da in dynamicAssets){
			if(!string.IsNullOrEmpty(da.url) && !urls.ContainsKey(da.url)) {
				urls.Add(da.url,true);
				groups.Add(CreateAsset(da.url,da.textureReadonly));
			}
		}
		ResManager.Instance.LoadGroup(groups.ToArray(),delegate(ResManager.Asset[] obj) {

			foreach(DynamicAsset da in dynamicAssets){
				da.LoadSprite();
			}
			if(onInited!=null) onInited();

		},delegate(ResManager.Asset[] assets, float pro) {

			if(onProgress!=null) onProgress(pro);

		});
	}


	#endregion



	/// <summary>
	/// 会自动加载
	/// </summary>
	public bool autoLoad = false;

	/// <summary>
	/// The texture readonly.
	/// </summary>
	public bool textureReadonly = true;

	[SerializeField]
	public string _url="";

	public string url {
		get{
			if(ResManager.IsInited() && _url.IndexOf(ResManager.Instance.searchPath)==0){
				_url = _url.Substring(ResManager.Instance.searchPath.Length);
			}
			return _url;
		}
		set{
			if(!_url.Equals(value))
			{
				_url = value;
				if(ResManager.IsInited() && _url.IndexOf(ResManager.Instance.searchPath)==0){
					_url = _url.Substring(ResManager.Instance.searchPath.Length);
				}
				if(_started || !autoLoad){
					if(string.IsNullOrEmpty(_url))
					{
						RemoveSprite();
					}
					else
					{
						LoadSprite();
					}
				}
				#if UNITY_EDITOR 
				if(!Application.isPlaying) UnityEditor.EditorUtility.SetDirty(this);
				#endif
			}
		}
	}

	/// <summary>
	/// 如果是图集，需要填名字
	/// </summary>
	[SerializeField]
	private string _frameName="";
	public string frameName{
		get{return _frameName;}
		set{
			if(!_frameName.Equals(value)){
				_frameName = value;
				if(string.IsNullOrEmpty(_frameName))
				{
					RemoveSprite();
				}
				else
				{
					ShowSprite();
				}
				#if UNITY_EDITOR 
				if(!Application.isPlaying) UnityEditor.EditorUtility.SetDirty(this);
				#endif
			}
		}
	}

	#region 替换材质上面的图片
	//是否是替换材质的贴图
	public bool replaceMatTexture;
	//要替换的材质
	public Material material;
	//要替换的贴图id
	public string nameId="_MainTex";
	//在disable时还原成之前的Texture
	public bool restoreOnDisable = false;
	//restoreOnDisable为ture时，要替换的材质
	public Texture prevTex;
	#endregion

	private static Sprite m_TransparentSprite = null;
	public static Sprite transparentSprite{
		get {
			if(m_TransparentSprite==null){
				Texture2D alphaTex = new Texture2D(2,2,TextureFormat.RGBA32,false);
				alphaTex.hideFlags = HideFlags.DontSaveInEditor|HideFlags.DontSaveInBuild;
				Color32 c = new Color32(255,255,255,0);
				alphaTex.SetPixels32(new Color32[]{
					c,c,c,c
				});
				alphaTex.Apply();
				m_TransparentSprite = Sprite.Create(alphaTex,new Rect(0,0,alphaTex.width,alphaTex.height),Vector2.zero*0.5f,100,0,SpriteMeshType.FullRect);
				m_TransparentSprite.hideFlags = HideFlags.DontSaveInEditor|HideFlags.DontSaveInBuild;
			}
			return m_TransparentSprite;
		}
	}
	private Image m_Image;


	private ResManager.Asset _asset;
	/// <summary>
	/// 当前资源
	/// </summary>
	/// <value>The asset.</value>
	public ResManager.Asset asset
	{
		get{return _asset; }
	}

	/// <summary>
	/// 初始化完成
	/// </summary>
	public UnityEngine.Events.UnityEvent onInited = new UnityEngine.Events.UnityEvent();

	//是否缓存加载的资源
	public bool cacheAsset = false;
	//Destroy里自动销毁Asset，需要确定这个asset是否没有其他对象在用
	public bool autoDisposeAsset = false;

	private bool _started = false;

	void Awake(){
		
		m_Image = GetComponent<Image>();
		if(m_Image && m_Image.sprite==null){
			m_Image.sprite = transparentSprite;
		}
	}

	// Use this for initialization
	protected virtual void Start () {
		_started = true;
		#if UNITY_EDITOR
		if(Application.isPlaying){
			if(autoLoad) LoadSprite();
		}else{
			LoadSprite();
		}
		#else
		if(autoLoad) LoadSprite();
		#endif
	}

	protected virtual void OnDisable(){
		if(replaceMatTexture && restoreOnDisable && material) {
			material.SetTexture(nameId,prevTex);
		}
	}

	/// <summary>
	/// 加载
	/// </summary>
	public virtual void LoadSprite()
	{
		if(!string.IsNullOrEmpty(_url))
		{
			if(Application.isPlaying){
				ResManager.Asset asset = ResManager.Instance.GetAsset(url);
				if(asset==null) 
				{
					asset = CreateAsset(_url,textureReadonly);
					ResManager.Instance.LoadAsset(asset,delegate(ResManager.Asset obj) {
						if(string.IsNullOrEmpty(_url)){
							RemoveSprite();
						}else if(asset.url.Equals(_url)){
							_asset = obj;
							if(_asset!=null && cacheAsset) _asset.cached = cacheAsset;
							ShowSprite();
							onInited.Invoke();
						}
					});
				}
				else
				{
					_asset = asset;
					if(_asset!=null && cacheAsset) _asset.cached = cacheAsset;
					ShowSprite();
					onInited.Invoke();
				}
			}else{
				#if UNITY_EDITOR
				ResManager.Asset asset = CreateAsset(url);
				LoadingAsset(asset,delegate(ResManager.Asset obj) {
					if(string.IsNullOrEmpty(_url)){
						RemoveSprite();
					}else if(asset.url.Equals(_url)){
						_asset = obj;
						ShowSprite();
						onInited.Invoke();
					}
				});
				#endif
			}
		}
		else
		{
			RemoveSprite();
		}
	}

	/// <summary>
	/// 通过StreamingAssets下面的路径创建asset
	/// </summary>
	/// <returns>The asset.</returns>
	/// <param name="url">URL.</param>
	/// <param name="textureReadonly">texture Readonly.</param>
	public static ResManager.Asset CreateAsset(string url,bool textureReadonly = true){
		ResManager.Asset asset = new ResManager.Asset();
		asset.meshType = SpriteMeshType.FullRect;
		asset.warpMode = TextureWrapMode.Clamp;
		asset.path= ResManager.AssetPath.StreamingAssets;
		asset.textureReadonly = textureReadonly;
		asset.url = url;
		if(url.LastIndexOf(".xml")>0){
			asset.type = ResManager.AssetType.Sprites;
		}else{
			asset.type = ResManager.AssetType.Sprite;
		}
		return asset;
	}

	protected virtual void ShowSprite(){
		if(_asset==null) return;

		if(replaceMatTexture)
		{
			if(material && _asset.texture!=null) {
				material.SetTexture(nameId,_asset.texture);
			}
		}
		else
		{
			if(m_Image)
			{
				if(_asset.sprites!=null && !string.IsNullOrEmpty(_frameName) && _asset.sprites.ContainsKey(_frameName)){
					m_Image.sprite = _asset.sprites[_frameName];
				}else if(url.ToLower().LastIndexOf(".xml")==-1){
					m_Image.sprite = _asset.sprite;
				}
				#if UNITY_EDITOR
				if(!Application.isPlaying) UnityEditor.EditorUtility.SetDirty (m_Image);
				#endif
			}
			else if(GetComponent<SpriteRenderer>())
			{
				SpriteRenderer sr = GetComponent<SpriteRenderer>();
				if(sr){
					if(_asset.sprites!=null && !string.IsNullOrEmpty(_frameName) && _asset.sprites.ContainsKey(_frameName)){
						sr.sprite = _asset.sprites[_frameName];
					}else if(url.ToLower().LastIndexOf(".xml")==-1){
						sr.sprite = _asset.sprite;
					}
				}
			}
		}
	}


	public virtual void RemoveSprite(bool disposeAsset = false)
	{
		if(replaceMatTexture)
		{
			if(material) {
				if(restoreOnDisable) material.SetTexture(nameId,prevTex);
			}
		}
		else
		{
			if(m_Image)
			{
				m_Image.sprite = transparentSprite;
				#if UNITY_EDITOR
				if(!Application.isPlaying) UnityEditor.EditorUtility.SetDirty (m_Image);
				#endif
			}
			else if( GetComponent<SpriteRenderer>())
			{
				SpriteRenderer sr = GetComponent<SpriteRenderer>();	
				sr.sprite = null;
			}
		}
		if(disposeAsset && _asset!=null && ResManager.IsInited()){
			#if !UNITY_EDITOR
			ResManager.Instance.DisposeAsset(_asset);
			#endif
			_asset = null;
		}
	}

	protected virtual void OnDestroy(){
		if(Application.isPlaying && !cacheAsset && autoDisposeAsset){
			RemoveSprite(true);
		}
	}


	#region Editor Load Asset

	#if UNITY_EDITOR
	protected virtual void LateUpdate(){
		if (m_Image!=null && m_Image.sprite==null){
			m_Image.sprite = transparentSprite;
		}
		if(replaceMatTexture && material && _asset!=null){
			material.SetTexture(nameId,_asset.texture);
		}
	}

	void LoadingAsset (ResManager.Asset asset, System.Action<ResManager.Asset> onLoaded)
	{
		if( asset.url.LastIndexOf(".xml")>0)
		{
			string xmlUrl = Application.streamingAssetsPath + "/" + asset.url;
			if(File.Exists(xmlUrl) && asset.type== ResManager.AssetType.Sprites){
				string atlasPath = asset.url.Substring (0, asset.url.LastIndexOf (".xml")) + ".png";
				string pngUrl = Application.streamingAssetsPath + "/" + atlasPath;
				LoadPng(pngUrl,asset,null);

				//load xml
				string config = File.ReadAllText(xmlUrl);
				XmlDocument xmlDoc = new XmlDocument ();
				xmlDoc.LoadXml (config);
				XmlNode root = xmlDoc.SelectSingleNode ("TextureAtlas");
				XmlNodeList nodeList = root.ChildNodes;

				asset.sprites = new Dictionary<string, Sprite> ();
				//遍历所有子节点
				foreach (XmlNode xn in nodeList) {
					if (!(xn is XmlElement))
						continue;
					XmlElement xe = (XmlElement)xn;
					string fn = xe.GetAttribute ("name").Replace ('/', '_');
					float x = float.Parse (xe.GetAttribute ("x"));
					float y = float.Parse (xe.GetAttribute ("y"));
					float w = float.Parse (xe.GetAttribute ("width"));
					float h = float.Parse (xe.GetAttribute ("height"));
					Sprite s = Sprite.Create (asset.texture, new Rect (x, asset.texture.height - h - y, w, h), Vector2.one * 0.5f, 100, 1, asset.meshType);
					s.hideFlags = HideFlags.DontSaveInEditor|HideFlags.DontSaveInBuild;
					s.name = fn;
					asset.sprites [fn] = s;
				}
				if (onLoaded != null) onLoaded (asset);
			}
		}
		else
		{
			string url = Application.streamingAssetsPath + "/" + asset.url;
			LoadPng(url,asset,onLoaded);
		}
	}

	void LoadPng(string url, ResManager.Asset asset,System.Action<ResManager.Asset> onLoaded){

		Texture2D texture = new Texture2D (2, 2,TextureFormat.RGBA32,false);
		texture.hideFlags = HideFlags.DontSaveInEditor|HideFlags.DontSaveInBuild;
		byte[] bytes;
		if(File.Exists(url+".txt"))
		{
			bytes = ResManager.ReverseBytes(File.ReadAllBytes(url+".txt"));
		}else if(File.Exists(url)){
			bytes = File.ReadAllBytes(url);
		}
		else
		{
			print("Load error : "+url);
			return;
		}
		texture.LoadImage (bytes, asset.textureReadonly);
		asset.texture = texture;
		asset.texture.name = url.Substring(url.LastIndexOf("/")+1);

		if (asset.type == ResManager.AssetType.Sprite) {
			asset.texture.wrapMode = asset.warpMode;
			asset.sprite = Sprite.Create (asset.texture, new Rect (0f, 0f, asset.texture.width, asset.texture.height), Vector2.one * 0.5f, 100, 1, asset.meshType);
			asset.sprite.name = asset.texture.name;
			asset.sprite.hideFlags = HideFlags.DontSaveInEditor|HideFlags.DontSaveInBuild;
			if (onLoaded != null) onLoaded (asset);
		} else if (asset.type == ResManager.AssetType.Texture2D) {
			asset.texture.wrapMode = asset.warpMode;
			if (onLoaded != null) onLoaded (asset);
		} 
	}

	#endif

	#endregion
}
