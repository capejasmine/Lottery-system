using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class USprite : MonoBehaviour {

	private MaterialPropertyBlock m_Block = null;

	private Sprite __Sprite;
	[SerializeField]
	private Sprite m_Sprite;
	public Sprite sprite{
		get { return m_Sprite; }
		set {
			if(m_Sprite!=value || __Sprite!=value){
				m_Sprite = value;
				__Sprite = value;
				if(m_Sprite){
					if(m_Block==null) m_Block = new MaterialPropertyBlock();
					m_Block.SetTexture("_MainTex",value.texture);
					meshRenderer.SetPropertyBlock(m_Block);
				}
				UpdateMeshAndUV();
				m_ColorIsDirty = true;
			}
		}
	}

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
		}
	}

	private Color __color = Color.white;
	[SerializeField]
	private Color m_Color = Color.white;
	public Color color {
		get { return m_Color;}
		set {
			if(!m_Color.Equals(value) || !__color.Equals(value)){
				m_Color = value;
				__color = value;
				m_ColorIsDirty=true;
			}
		}
	}

	private MeshRenderer m_MeshRenderer;
	public MeshRenderer meshRenderer{
		get{ 
			if(m_MeshRenderer==null) m_MeshRenderer = gameObject.GetComponent<MeshRenderer>();
			m_MeshRenderer.hideFlags = HideFlags.HideInInspector;
			return m_MeshRenderer; 
		}
	}

	private MeshFilter m_MeshFilter;
	public MeshFilter meshFilter{
		get{
			if(m_MeshFilter==null) m_MeshFilter = gameObject.GetComponent<MeshFilter>();
			m_MeshFilter.hideFlags = HideFlags.HideInInspector;
			return m_MeshFilter;
		}
	}

	[SerializeField]
	private Material m_Material;
	public Material material{
		get{ return m_Material ; }
		set{
			m_Material = value;
			meshRenderer.sharedMaterial = value; 
		}
	}

	private Vector2 __m_Pivot = new Vector2(0.5f,0.5f) ;
	[SerializeField]
	private Vector2 m_Pivot = new Vector2(0.5f,0.5f) ;
	public Vector2 pivot{
		get{ return m_Pivot; }
		set{
			m_Pivot.x = Mathf.Clamp01(m_Pivot.x);
			m_Pivot.y = Mathf.Clamp01(m_Pivot.y);
			if(!m_Pivot.Equals(value) || !__m_Pivot.Equals(value)){
				m_Pivot = value;
				__m_Pivot = value;
				m_VertexIsDirty=true;
			}
		}
	}

	private float __skiewX= 0f;
	[SerializeField]
	[Range(-2f,2f)]
	private float m_skewX = 0f;
	public float skewX{
		get{ return m_skewX; }
		set {
			if(value>=-2f && value<=2f && (m_skewX!=value||__skiewX!=value)){
				m_skewX = value;
				__skiewX = value;
				m_VertexIsDirty=true;
			}
		}
	}


	private float __skewY = 0f;
	[SerializeField]
	[Range(-2f,2f)]
	private float m_skewY=0f;
	public float skewY{
		get{ return m_skewY; }
		set {
			if(value>=-2f && value<=2f && (m_skewY!=value || __skewY!=value)){
				m_skewY = value;
				__skewY = value;
				m_VertexIsDirty=true;
			}
		}
	}

	private bool __RotatePerspective = true;
	[SerializeField]
	private bool m_RotatePerspective = true ;
	public bool rotatePerspective{
		get { return m_RotatePerspective;}
		set{
			if(m_RotatePerspective!=value || __RotatePerspective!=value){
				__RotatePerspective = value;
				m_RotatePerspective = value;
				m_VertexIsDirty=true;
			}
		}
	}

	private float __PerspectiveRange=1f;
	[Range(0f,2f)]
	[SerializeField]
	private float m_PerspectiveRange = 1f;
	public float perspectiveRange{
		get{ return m_PerspectiveRange;}
		set{
			if(m_PerspectiveRange!=value || __PerspectiveRange!=value){
				__PerspectiveRange = value;
				m_PerspectiveRange =value;
				m_VertexIsDirty=true;
			}
		}
	}

	private Mesh m_Mesh ;
	private bool m_VertexIsDirty = false;
	private bool m_ColorIsDirty = false;
	private float m_RotateX = 0f;
	private float m_RotateY = 0f;

	void UpdateMeshAndUV(){
		if(!m_Mesh){
			m_Mesh = new Mesh();
		}

		UpdateVertex();

		if(m_Sprite){
			m_Mesh.uv=new Vector2[]{
				new Vector2((m_Sprite.rect.x+m_Sprite.rect.width)/m_Sprite.texture.width,(m_Sprite.rect.y+m_Sprite.rect.height)/m_Sprite.texture.height),
				new Vector2((m_Sprite.rect.x+m_Sprite.rect.width)/m_Sprite.texture.width,m_Sprite.rect.y/m_Sprite.texture.height),
				new Vector2(m_Sprite.rect.x/m_Sprite.texture.width,m_Sprite.rect.y/m_Sprite.texture.height),
				new Vector2(m_Sprite.rect.x/m_Sprite.texture.width,(m_Sprite.rect.y+m_Sprite.rect.height)/m_Sprite.texture.height)
			};
		}
		m_Mesh.triangles=new int[]{0,1,2,2,3,0};
		m_Mesh.RecalculateBounds();
		m_Mesh.RecalculateNormals();

		meshFilter.sharedMesh = m_Mesh;
	}

	private Vector2 size{
		get { return m_Sprite? new Vector2(m_Sprite.rect.width,m_Sprite.rect.height) : Vector2.zero; }
	}

	void UpdateVertex(){
		if(m_Mesh){
			Vector2 spriteSize = size*0.01f;
			float topSkewX = m_skewX*spriteSize.x*(1f-m_Pivot.y);
			float bottomSkewX = m_skewX*spriteSize.x*m_Pivot.y;
			float leftSkewX = m_skewY*spriteSize.y*m_Pivot.x;
			float rightSkewY = m_skewY*spriteSize.y*(1f-m_Pivot.x);
			float offX = spriteSize.x*m_Pivot.x;
			float offY = spriteSize.y*m_Pivot.y;

			float persX = 0f,persY = 0f;
			if(m_RotatePerspective){
				float ry = Mathf.Sin(transform.localEulerAngles.y*Mathf.Deg2Rad);
				float rx = Mathf.Sin(transform.localEulerAngles.x*Mathf.Deg2Rad);
				persY = ry*spriteSize.y*0.05f*m_PerspectiveRange;
				persX = rx*spriteSize.x*0.05f*m_PerspectiveRange;
			}
			m_Mesh.vertices = new Vector3[]{
				new Vector3(spriteSize.x+topSkewX-offX-persX ,spriteSize.y+rightSkewY-offY+persY,0f),
				new Vector3(spriteSize.x-bottomSkewX-offX+persX,rightSkewY-offY-persY,0f),
				new Vector3(-bottomSkewX-offX-persX ,-leftSkewX-offY+persY,0f ),
				new Vector3(topSkewX-offX+persX ,spriteSize.y-leftSkewX-offY-persY,0f )
			};
		}
	}
	void UpdateVertexColor(){
		if(m_Mesh){
			if(m_Mesh.colors==null || m_Mesh.colors.Length!=4){
				m_Mesh.colors = new Color[4]{Color.white,Color.white,Color.white,Color.white};
			}
			Color[] colors = m_Mesh.colors;
			for(int i=0;i<colors.Length;++i){
				colors[i] = m_Color;
			}
			m_Mesh.colors = colors;
		}
	}

	// Use this for initialization
	void Start () {
		m_RotateX = transform.localEulerAngles.x;
		m_RotateY = transform.localEulerAngles.y;
	}

	// Update is called once per frame
	void LateUpdate () {
		sprite = m_Sprite;
		material = m_Material;
		color = m_Color;
		skewX = m_skewX;
		skewY = m_skewY;
		m_Pivot.x = Mathf.Clamp01(m_Pivot.x);
		m_Pivot.y = Mathf.Clamp01(m_Pivot.y);
		pivot = m_Pivot;
		rotatePerspective = m_RotatePerspective;
		if(rotatePerspective){
			perspectiveRange = m_PerspectiveRange;

			if(transform.localEulerAngles.x!=m_RotateX || transform.localEulerAngles.y!=m_RotateY){
				m_RotateX = transform.localEulerAngles.x;
				m_RotateY = transform.localEulerAngles.y;
				m_VertexIsDirty = true;
			}
		}

		if(m_VertexIsDirty){
			UpdateVertex();
			m_VertexIsDirty = false;
		}
		if(m_ColorIsDirty){
			UpdateVertexColor();
			m_ColorIsDirty = false;
		}
	}
}