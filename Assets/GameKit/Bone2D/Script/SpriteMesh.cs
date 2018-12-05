﻿using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bones2D
{
	/// <summary>
	/// Sprite mesh.
	/// author:bingheliefeng
	/// </summary>
	[ExecuteInEditMode,DisallowMultipleComponent,RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
	public class SpriteMesh : MonoBehaviour {
		
		public Vector3[] vertices;
		public Vector2[] uvs;
		public Color[] colors;//vertex color
		public int[] triangles;
		public int[] edges;

		public Armature.BoneWeightClass[] weights;//for skinnedMesh
		public Transform[] bones;
		public Matrix4x4[] bindposes; //for skinnedMesh
		public Transform[] vertControlTrans;//for ffd animation

		//	public Vector2 pivot;

		[HideInInspector]
		[SerializeField]
		private TextureFrame m_Frame;
		public TextureFrame frame{
			get { return m_Frame; }
			set {
				if(m_Frame!=value){
					m_Frame = value;
					if(m_Frame!=null){
						UpdateUV();
						if(Application.isPlaying){
							if(material==null){
								material = m_Frame.material;
							}
						}else{
							material = m_Frame.material;
						}
					}
				}
			}
		}

//		[SerializeField]
//		private Vector2 m_uvOffset;
//		public Vector2 uvOffset{
//			get { return m_uvOffset; }
//			set{
//				m_uvOffset = value;
//				if(m_createdMesh) UpdateUV();
//			}
//		}

		[Range(0f,1f)]
		[SerializeField]
		private float m_brightness = 0f;
		public float brightness{
			get { return m_brightness;}
			set {	m_brightness=value;
				if(m_createdMesh)	UpdateVertexColor();
			}
		}

		private bool m_ColorIsDirty = false;
		[SerializeField]
		private Color m_Color = Color.white;//for animation
		public Color color{
			get { return m_Color;}
			set { 
				if(m_Color.a!=value.a || m_Color.r != value.r || m_Color.g != value.g || m_Color.b != value.b){
					m_Color = value; 
					m_ColorIsDirty = true;
				}
			}
		}
		[SerializeField]
		protected internal bool m_PreMultiplyAlpha = false;

		[SerializeField]
		private string m_sortingLayerName="Default";
		public string sortingLayerName{
			get { return m_sortingLayerName;}
			set {	m_sortingLayerName=value;
				if(m_createdMesh)	UpdateSorting();
			}
		}

		[SerializeField]
		private int m_sortingOrder = 0;
		public int sortingOrder{
			get { return m_sortingOrder;}
			set {	m_sortingOrder=value;
				if(m_createdMesh)	UpdateSorting();
			}
		}

		public Material material{
			get {
				if(m_render) return m_render.sharedMaterial;
				return null;
				}
			set{
				if(m_render) m_render.sharedMaterial= value;
			}
		}

		[HideInInspector]
		[SerializeField]
		private bool m_createdMesh;
		public bool isCreatedMesh{
			get{
				return m_createdMesh && m_mesh!=null;
			}
		}

		[HideInInspector]
		[SerializeField]
		private Mesh m_mesh;
		public Mesh mesh{
			get {return m_mesh;}
		}
		private Renderer m_render;
		public Renderer render{
			get{ 
				if(m_render==null){
					m_render = GetComponent<MeshRenderer>();
					if(m_render==null){
						m_render = gameObject.AddComponent<MeshRenderer>();
					}
				}
				return m_render;
			}
		}


		[HideInInspector]
		[SerializeField]
		private PolygonCollider2D m_collder;

		[HideInInspector]
		[SerializeField]
		private Vector2[] m_collider_points;

		private Vector3[] m_weightVertices;

		#if UNITY_EDITOR
		public Vector3[] weightVertices{
			get{  return m_weightVertices; }
		}
		public bool isEdit = false;
		#endif

		void OnEnable(){
			if(m_createdMesh && m_mesh==null){
				this.CreateMesh();
			}
			m_ColorIsDirty = true;
			UpdateMesh();
			m_ColorIsDirty = false;
		}

		public void CreateMesh(){
			if(vertices!=null && uvs!=null && colors!=null && triangles!=null){
				if(m_mesh==null) {
					m_mesh = new Mesh();
					m_mesh.hideFlags = HideFlags.DontSaveInEditor|HideFlags.DontSaveInBuild;
					m_mesh.MarkDynamic();
				}
				m_mesh.vertices = vertices;
				UpdateUV();
				m_mesh.triangles = triangles;
				if(edges!=null && edges.Length>0 && m_collder==null){
					m_collder = GetComponent<PolygonCollider2D>();
					if(m_collder==null) m_collder = gameObject.AddComponent<PolygonCollider2D>();
					UpdateEdges();
				}
				UpdateVertexColor();
				m_mesh.RecalculateBounds();

				render.receiveShadows = false;
				render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

				MeshFilter mf = GetComponent<MeshFilter>();
				if(mf==null) mf = gameObject.AddComponent<MeshFilter>();
				mf.sharedMesh = m_mesh;

				UpdateSorting();
				m_createdMesh = true;
			}
		}


		public void UpdateUV(){
			if(m_mesh && frame!=null && uvs!=null){
				//uv to Atlas
				Vector2[] tempUvs = new Vector2[uvs.Length] ;
				for(int i=0;i<uvs.Length;++i){
					Vector2 uv=uvs[i];
					if(frame.isRotated){
						float x = uv.y*frame.rect.width;
						float y = frame.rect.height - uv.x*frame.rect.height;
						uv.x = 1-x/frame.rect.width;
						uv.y = 1-y/frame.rect.height;
					}
					Vector2 uvPos = new Vector2(frame.rect.x,frame.atlasTextureSize.y-frame.rect.y-frame.rect.height)+ 
						new Vector2(frame.rect.width*uv.x,frame.rect.height*uv.y);
					uv.x = uvPos.x/frame.atlasTextureSize.x;
					uv.y = uvPos.y/frame.atlasTextureSize.y;
					tempUvs[i] = uv;
				}
//				for(int i=0;i<tempUvs.Length;++i){
//					tempUvs[i] = tempUvs[i]-m_uvOffset;
//				}
				m_mesh.uv = tempUvs;
			}
		}

		public void UpdateVertexColor(){
			if(m_mesh){
				Color col = m_Color;
				col *= (Mathf.Clamp(m_brightness,0f,1f)*2f+1f);
				if(m_PreMultiplyAlpha){
					col.r*=col.a;
					col.g*=col.a;
					col.b*=col.a;
				}
				for(int i=0;i<colors.Length;++i){
					colors[i]=col;
				}
				m_mesh.colors=colors;
			}
		}

		public void UpdateSorting(){
			render.sortingLayerName = m_sortingLayerName;
			render.sortingOrder = m_sortingOrder;
		}

		#if UNITY_EDITOR
		void Update(){
			if(!Application.isPlaying && m_createdMesh && m_mesh && vertControlTrans!=null){
				int len = vertControlTrans.Length;
				for(int i=0;i<len;++i){
					vertices[i] = vertControlTrans[i].localPosition;
				}
				m_mesh.vertices=vertices;
				UpdateUV();
				m_mesh.triangles=triangles;
				UpdateEdges();
				UpdateVertexColor();
				UpdateSorting();
				m_mesh.RecalculateBounds();
			}
		}
		#endif

		public void UpdateMesh(){
			if(m_createdMesh && m_mesh){
				if(vertControlTrans!=null){
					int len = vertControlTrans.Length;
					for(int i=0;i<len;++i){
						vertices[i] = vertControlTrans[i].localPosition;
					}
				}
				if(m_ColorIsDirty){
					UpdateVertexColor();
					m_ColorIsDirty = false;
				}
				UpdateEdges();
				UpdateSkinnedMesh();
				m_mesh.RecalculateBounds();
			}
		}


		private Matrix4x4[] m_BoneMatrices = null;
		public void UpdateSkinnedMesh(){
			if(weights!=null && weights.Length>0)
			{
				int len = vertices.Length;
				int boneLen = bones.Length;
				if(m_weightVertices==null){
					m_weightVertices = new Vector3[len];
				}
				if(m_BoneMatrices==null || m_BoneMatrices.Length!=boneLen){
					m_BoneMatrices = new Matrix4x4[boneLen];
				}
				for (int j= 0; j< boneLen; ++j){
					m_BoneMatrices[j] = bones[j].localToWorldMatrix * bindposes[j];
				}
				Matrix4x4 vertexMatrix = new Matrix4x4();
				Matrix4x4 worldMat = transform.worldToLocalMatrix;
				for(int i=0;i<len;++i){
					Vector3 v = vertices[i]; //local vertex
					Armature.BoneWeightClass bw = weights[i];
					Matrix4x4 bm0 = m_BoneMatrices[bw.boneIndex0];
					Matrix4x4 bm1 = m_BoneMatrices[bw.boneIndex1];
					Matrix4x4 bm2 = m_BoneMatrices[bw.boneIndex2];
					Matrix4x4 bm3 = m_BoneMatrices[bw.boneIndex3];
					Matrix4x4 bm4 = m_BoneMatrices[bw.boneIndex4];

					for (int n= 0; n < 16; ++n){
						vertexMatrix[n] =
							bm0[n] * bw.weight0 +
							bm1[n] * bw.weight1 +
							bm2[n] * bw.weight2 +
							bm3[n] * bw.weight3 +
							bm4[n] * bw.weight4;
					}
					vertexMatrix = worldMat*vertexMatrix;
					v = vertexMatrix.MultiplyPoint3x4(v);
					v.z=0f;
					m_weightVertices[i] = v;
				}
				m_mesh.vertices = m_weightVertices;
			}
			else
			{
				m_mesh.vertices = vertices;
			}
		}

		public void UpdateEdges(){
			if(m_collder!=null && edges!=null && edges.Length>0){
				int len = edges.Length;
				if(m_collider_points==null) {
					m_collider_points = new Vector2[len];
				}
				Matrix4x4 worldMat = transform.worldToLocalMatrix;
				Matrix4x4 vertexMatrix = new Matrix4x4();
				for(int i=0;i<len;++i){
					int vIndex = edges[i];
					Vector3 v = vertices[vIndex]; //local vertex

					if(weights!=null && weights.Length>0){
						Armature.BoneWeightClass bw = weights[vIndex];
						Matrix4x4 bm0 = m_BoneMatrices[bw.boneIndex0];
						Matrix4x4 bm1 = m_BoneMatrices[bw.boneIndex1];
						Matrix4x4 bm2 = m_BoneMatrices[bw.boneIndex2];
						Matrix4x4 bm3 = m_BoneMatrices[bw.boneIndex3];
						Matrix4x4 bm4 = m_BoneMatrices[bw.boneIndex4];

						for (int n= 0; n < 16; ++n){
							vertexMatrix[n] =
								bm0[n] * bw.weight0 +
								bm1[n] * bw.weight1 +
								bm2[n] * bw.weight2 +
								bm3[n] * bw.weight3 +
								bm4[n] * bw.weight4;
						}
						vertexMatrix = worldMat*vertexMatrix;
						v = vertexMatrix.MultiplyPoint3x4(v);
						v.z=0f;
					}
					m_collider_points[i] = (Vector2)v;
				}
				m_collder.points = m_collider_points;
			}
		}


		#if UNITY_EDITOR
		void OnDrawGizmos(){
			if(Selection.activeTransform!=null && m_mesh  && m_mesh.vertexCount>0){
				if(Selection.activeTransform==this.transform|| Selection.activeTransform.parent==this.transform){
					Gizmos.color = Color.red;
					Vector3[] vs = (m_weightVertices==null||m_weightVertices.Length==0) ? vertices : m_weightVertices ;
					foreach(Vector3 v in vs){
						Gizmos.DrawWireSphere (transform.TransformPoint (v), 0.02f);
					}
				}
			}
		}
		#endif
	}
}