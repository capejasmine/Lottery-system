using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityEngine.UI
{
	[ExecuteInEditMode, RequireComponent(typeof(CanvasRenderer), typeof(RectTransform))]
	public class USpriteUGUI : MaskableGraphic {

		private Sprite __Sprite;
		[SerializeField]
		private Sprite m_Sprite;
		public Sprite sprite{
			get { return m_Sprite; }
			set {
				if(m_Sprite!=value || __Sprite!=value){
					m_Sprite = value;
					__Sprite = value;
					if(value){
						rectTransform.sizeDelta = new Vector2(m_Sprite.rect.width,m_Sprite.rect.height);
					}
					UpdateMeshAndUV();
				}
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

		public override Texture mainTexture {
			get {
				return m_Sprite == null ? s_WhiteTexture : m_Sprite.texture;
			}
		}

		private Mesh m_Mesh ;
		private bool m_VertexIsDirty = false;
		private float m_RotateX = 0f;
		private float m_RotateY = 0f;

		protected override void Start(){
			base.Start();
			m_RotateX = transform.localEulerAngles.x;
			m_RotateY = transform.localEulerAngles.y;
			_Update();
		}

		public override void Rebuild (CanvasUpdate update) {
			base.Rebuild(update);
			if (canvasRenderer.cull) return;
			if (update == CanvasUpdate.PreRender){
				_Update();
			}
		}

		protected override void OnPopulateMesh (VertexHelper vh)
		{
			vh.Clear();
		}

		void _Update(){
			sprite = m_Sprite;
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
			UpdateVertexColor();

			canvasRenderer.SetMesh(m_Mesh);
		}
		void LateUpdate(){
			if(m_Mesh)
				rectTransform.sizeDelta = (Vector2)m_Mesh.bounds.size;
		}

		private Vector2 size{
			get { return m_Sprite? new Vector2(m_Sprite.rect.width,m_Sprite.rect.height) : Vector2.zero; }
		}

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
			canvasRenderer.SetMesh(m_Mesh);
		}

		void UpdateVertexColor(){
			if(m_Mesh){
				if(m_Mesh.colors==null || m_Mesh.colors.Length!=4){
					m_Mesh.colors32 = new Color32[4]{Color.white,Color.white,Color.white,Color.white};
				}
				Color32[] colors = m_Mesh.colors32;
				for(int i=0;i<colors.Length;++i){
					colors[i] = color;
				}
				m_Mesh.colors32 = colors;
			}
		}

		void UpdateVertex(){
			if(m_Mesh){
				Vector2 spriteSize = size;
				float topSkewX = m_skewX*spriteSize.x*(1f-pivot.y);
				float bottomSkewX = m_skewX*spriteSize.x*pivot.y;
				float leftSkewX = m_skewY*spriteSize.y*pivot.x;
				float rightSkewY = m_skewY*spriteSize.y*(1f-pivot.x);
				float offX = spriteSize.x*pivot.x;
				float offY = spriteSize.y*pivot.y;

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
				m_Mesh.RecalculateBounds();
			}
		}
	}
}