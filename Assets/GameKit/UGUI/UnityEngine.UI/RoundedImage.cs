using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
	/// <summary>
	/// 圆角图片
	/// </summary>
	[AddComponentMenu("UI/Rounded Image"),ExecuteInEditMode,DisallowMultipleComponent,RequireComponent(typeof(CanvasRenderer), typeof(RectTransform))]
	public class RoundedImage : MaskableGraphic {


		[SerializeField]
		private Texture m_Texture;
		public override Texture mainTexture {
			get { 
				return m_Texture ;//== null ? material.mainTexture : m_Texture;
			}
		}
		/// <summary>
		/// Texture to be used.
		/// </summary>
		public Texture texture
		{
			get{
				return m_Texture;
			}
			set
			{
				if (m_Texture == value)
					return;
				m_Texture = value;
				SetMaterialDirty();
			}
		}

		[SerializeField]
		private Vector2 m_Size = new Vector2(500,500);
		public Vector2 Size{
			get{
				return m_Size;
			}
			set{
				if(m_Size.x!=value.x || m_Size.y!=value.y){
					m_Size = value;
					SetVerticesDirty();
				}
			}
		}

		[Range(0f,1f)]
		public float RoundEdges = 0.25f;
		public bool enableLeftTop = true;
		public bool enableRightTop = true;
		public bool enableLeftBottom = true;
		public bool enableRightBottom = true;
		[Range(2,32)]
		public int CornerVertexCount = 8;
		public bool CreateUV = true;

		private MeshFilter m_MeshFilter;
		private MeshRenderer m_Renderer;
		private Mesh m_Mesh;
		private Vector3[] m_Vertices;
		private Color32[] m_Colors;
		private Vector2[] m_UV;
		private int[] m_Triangles;

		public override void Rebuild (CanvasUpdate update) {
			base.Rebuild(update);
			if (canvasRenderer.cull) return;
			if (update == CanvasUpdate.PreRender){
				UpdateMesh();
			}
		}

		protected override void OnPopulateMesh (VertexHelper vh)
		{
			vh.Clear();
		}

		public void UpdateMesh()
		{
			var trans = transform as RectTransform;
			float w = m_Size.x*0.5f;
			float h = m_Size.y*0.5f;
			if(w<0 || h < 0 ){
				w = trans.sizeDelta.x;
				h = trans.sizeDelta.y;
			}

			Vector2 half = Vector2.one*0.5f;

			if (CornerVertexCount<2)
				CornerVertexCount = 2;
			int vCount = CornerVertexCount * 4 + 1 ;
			int triCount = (CornerVertexCount * 4) ;
			if (m_Vertices == null || m_Vertices.Length != vCount)
			{
				m_Vertices = new Vector3[vCount];
				m_Colors = new Color32[vCount];
			}
			if (m_Triangles == null || m_Triangles.Length != triCount * 3)
				m_Triangles = new int[triCount * 3];
			if (CreateUV && (m_UV == null || m_UV.Length != vCount))
			{ 
				m_UV = new Vector2[vCount];
			}
			float f = 1f / (CornerVertexCount-1);

			Vector2 vOffset = Vector2.zero;
			Vector2 pivot = Vector2.one*0.5f - trans.pivot;
			vOffset.x = w*pivot.x*2f;
			vOffset.y = h*pivot.y*2f;

			m_Vertices[0] = vOffset;
			int count = CornerVertexCount * 4;
			if (CreateUV)
			{
				m_UV[0] = half;
			}

			float re = RoundEdges*100f;
			for (int i = 0; i < CornerVertexCount; ++i )
			{
				float v = (float)i * Mathf.PI * 0.5f*f;
				float s = Mathf.Sin(v);
				float c = Mathf.Cos(v);

				Vector2 v1 = new Vector2(-w,h);
				if(enableLeftTop){
					v1 = new Vector2(-w + re - c * re, - re + s * re+h);
				}
				Vector2 v2 = new Vector2(w,h);
				if(enableRightTop){
					v2 = new Vector2(w - re + s * re,  - re + c * re+h);
				}
				Vector2 v3 = new Vector2(w,-h);
				if(enableRightBottom){
					v3 = new Vector2(w - re + c * re,  re - s * re-h);
				}
				Vector2 v4 = new Vector2(-w,-h);
				if(enableLeftBottom){
					v4 = new Vector2(-w + re - s * re, re - c * re-h);
				}

				m_Vertices[1 + i] = v1+vOffset;
				m_Vertices[1 + CornerVertexCount + i] = v2+vOffset;
				m_Vertices[1 + CornerVertexCount * 2 + i] = v3+vOffset;
				m_Vertices[1 + CornerVertexCount * 3 + i] = v4+vOffset;
				if (CreateUV)
				{

					v1.x/=w;
					v1.y/=h;

					v2.x/=w;
					v2.y/=h;

					v3.x/=w;
					v3.y/=h;

					v4.x/=w;
					v4.y/=h;

					m_UV[1 + i] = (v1*0.5f+half);
					m_UV[1 + CornerVertexCount * 1 + i] = (v2*0.5f+half);
					m_UV[1 + CornerVertexCount * 2 + i] = (v3*0.5f+half);
					m_UV[1 + CornerVertexCount * 3 + i] = (v4*0.5f+half);
				}
			}
			Color32 cc = color;
			for (int i = 0; i < count + 1;++i )
			{
				m_Colors[i] = cc;
			}

			for (int i = 0; i < count; ++i)
			{
				m_Triangles[i*3    ] = 0;
				m_Triangles[i*3 + 1] = i + 1;
				m_Triangles[i*3 + 2] = i + 2;
			}
			m_Triangles[count * 3 - 1] = 1;

			if(m_Mesh==null) {
				m_Mesh = new Mesh();
				m_Mesh.hideFlags = HideFlags.DontSaveInEditor|HideFlags.DontSaveInBuild;
				m_Mesh.MarkDynamic();
			}
			m_Mesh.Clear();
			m_Mesh.vertices = m_Vertices;
			if (CreateUV)
				m_Mesh.uv = m_UV;
			m_Mesh.triangles = m_Triangles;
			m_Mesh.colors32 = m_Colors;
			canvasRenderer.SetMesh(m_Mesh);
		}

		#if UNITY_EDITOR
		void LateUpdate(){
			if(!Application.isPlaying){
				UpdateMesh();
			}
		}
		#endif


	}
}