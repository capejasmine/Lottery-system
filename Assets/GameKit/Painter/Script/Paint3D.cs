using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Paint3D : MonoBehaviour {

	#region enums
	public enum PaintType
	{
		Scribble,
		DrawLine,
		DrawColorfulLine,
		None = 100
	} 
	#endregion

	[Header("Painter Setting")]
	public PaintType paintType = PaintType.Scribble;
	public bool useVectorGraphic = true; //矢量GL绘图,性能更高，但是不支持画笔贴图的alpha通道


	[SerializeField]
	private bool m_renderTexMipmap = false;

	[SerializeField]
	private Texture m_sourceTex; //scribble source texture
	public Texture sourceTex{
		get{ return m_sourceTex; }
		set{ 
			m_sourceTex = value; 
			if(canvasMat2){
				canvasMat2.SetTexture("_SourceTex",m_sourceTex);
			}else{
				canvasMat.SetTexture("_SourceTex",m_sourceTex);
			}
		}
	}

	[SerializeField]
	private int m_renderTexWidth = 256;
	[SerializeField]
	private int m_renderTexHeight = 256;

	[SerializeField]
	private Color m_penColor=new Color(1, 1, 1, 1);

	[Range(0.1f,5f)]
	public float brushScale = 1f;//change pen size

	[Range(0.01f,2f)]
	public float drawLerpDamp = 0.02f; //line continous

	[SerializeField]
	private bool m_isEraser = false;


	[Header("Colorfull paint Setting")]
	public Color[] paintColorful ;
	[Range(0f,10f)]
	public float colorChangeRate = 1f;


	[Header("Auto Setting")]
	public bool isAutoInit = false;
	public bool isAutoDestroy = true;//Destroy renderTexture when gameobject is destroyed.
	public bool isShowSource = false;


	[Header("Material")]
	public Material penMat;
	public Material canvasMat;
	public Material canvasMat2; //用于makeup

	[SerializeField]
	private RenderTexture m_rt,m_rt2;
	[HideInInspector]
	[SerializeField]
	private bool m_inited = false;
	public bool isInited{
		get{
			return m_inited;
		}
	}

	public RenderTexture renderTexture{
		get{ return m_rt; }
		set{ 
			m_rt = value; 
			if(canvasMat){
				canvasMat.SetTexture("_RenderTex",m_rt);
			}
		}
	}
	public RenderTexture renderTexture2{
		get{ return m_rt2; }
		set{ 
			m_rt2 = value; 
			if(canvasMat2){
				canvasMat2.SetTexture("_RenderTex",m_rt2);
			}
		}
	}

	private int m_colorfulIndex = 1;
	private Vector3 m_prevMousePosition;
	private float m_colorfulTime = 0f;
	private bool m_isDrawing = false;

	[Header("Mesh")]
	[SerializeField]
	private MeshCollider m_Collider;



	public bool isErase{
		get{ return m_isEraser; }
		set{
			if(m_isEraser!=value){
				m_isEraser = value;
				if(m_inited){
					penMat.SetFloat("_Cutoff",0f);
					if(m_isEraser){
						penMat.SetFloat("_BlendSrc",(int)BlendMode.Zero);
						penMat.SetFloat("_BlendDst",(int)BlendMode.OneMinusSrcAlpha);

					}else{
						penMat.SetFloat("_BlendSrc",(int)BlendMode.SrcAlpha);
						if(paintType== PaintType.DrawLine || paintType== PaintType.DrawColorfulLine){
							penMat.SetFloat("_BlendDst",(int)BlendMode.OneMinusSrcAlpha);
						}
						else if(paintType== PaintType.None){
							penMat.SetFloat("_BlendDst",(int)BlendMode.SrcAlpha);
						}
						else {
							penMat.SetFloat("_BlendDst",(int)BlendMode.One);
						}
					}
				}
			}
		}
	}



	void Start () {
		if(m_Collider){
			m_Collider = GetComponent<MeshCollider>();
		}
		if(m_Collider==null){
			Debug.LogError("MeshCollider 不能为null!");
			return;
		}

		if (isAutoInit) {
			Init();
		}
	}

	public void Init()
	{
		if(!m_inited){

			if(m_sourceTex==null){
				Debug.LogError("m_sourceTex 不能为null!");
				return;
			}

			m_inited = true;

			if(canvasMat2){
				if(m_rt==null){
					m_rt = new RenderTexture(m_renderTexWidth,m_renderTexHeight,0,RenderTextureFormat.ARGB32);
					m_rt.filterMode = FilterMode.Bilinear;
					m_rt.useMipMap = m_renderTexMipmap;
				}

				canvasMat.SetTexture("_MainTex",m_rt);

				if(m_rt2==null){
					m_rt2 = new RenderTexture(m_renderTexWidth,m_renderTexHeight,0,RenderTextureFormat.ARGB32);
					m_rt2.filterMode = FilterMode.Bilinear;
					m_rt2.useMipMap = m_renderTexMipmap;
					ClearCanvas(m_rt2);
				}

				canvasMat2.SetTexture("_SourceTex",m_sourceTex);
				canvasMat2.SetTexture("_RenderTex",m_rt2);

				penMat.SetFloat("_BlendSrc",(int)BlendMode.SrcAlpha);
				penMat.SetFloat("_BlendDst",(int)BlendMode.OneMinusSrcAlpha);

				canvasMat2.SetFloat("_BlendSrc",(int)UnityEngine.Rendering.BlendMode.One);
				canvasMat2.SetFloat("_BlendDst",(int)UnityEngine.Rendering.BlendMode.Zero);
			}
			else
			{
				if(m_rt==null){
					m_rt = new RenderTexture(m_renderTexWidth,m_renderTexHeight,0,RenderTextureFormat.ARGB32);
					m_rt.filterMode = FilterMode.Bilinear;
					m_rt.useMipMap = m_renderTexMipmap;
				}

				canvasMat.SetTexture("_SourceTex",m_sourceTex);
				canvasMat.SetTexture("_RenderTex",m_rt);

				if(isErase){
					isErase = false;
					isErase = true;
				}else{
					isErase = true;
					isErase = false;
				}
			}

			if(isShowSource){
				ShowTexture(m_sourceTex);
			}else{
				ResetCanvas();
			}
		}
	}

	/// <summary>
	/// 显示贴图
	/// </summary>
	/// <param name="texture">Texture.</param>
	public void ShowTexture(Texture texture){
		if(m_rt && texture){
			Graphics.SetRenderTarget (m_rt);
			Graphics.Blit(texture,m_rt);
			RenderTexture.active = null;
		}
	}

	/// <summary>
	/// draw when moving
	/// </summary>
	/// <param name="screenPos">Screen position.</param>
	/// <param name="camera">Camera.</param>
	/// <param name="canvasMaterial">Canvas Material.</param>
	public void Drawing(Vector3 screenPos , Camera camera=null, Material canvasMaterial = null){
		if(!m_inited) return;

//		if (camera == null) camera = Camera.main;
		if (canvasMaterial == null) canvasMaterial = this.canvasMat;

		RenderTexture rt = canvasMaterial.GetTexture("_RenderTex") as RenderTexture;
		if(rt==null) return;

		RaycastHit hit;
		if (!Physics.Raycast(camera.ScreenPointToRay(screenPos), out hit)){
			m_isDrawing = false;
			return;
		}
		if(hit.collider!=m_Collider) {
			m_isDrawing = false;
			return;
		}

		Vector3 uvPos= hit.textureCoord;
		screenPos = new Vector3(uvPos.x * rt.width, rt.height - uvPos.y * rt.height,0f);
		if(!m_isDrawing){
			m_isDrawing = true;
			m_prevMousePosition = screenPos;
		}

		if(m_isDrawing){
			if(paintType== PaintType.DrawColorfulLine){
				Color currC = paintColorful[m_colorfulIndex];
				m_penColor = Color.Lerp(m_penColor,currC,Time.deltaTime*colorChangeRate);
				m_colorfulTime+=Time.deltaTime*colorChangeRate;
				if(m_colorfulTime>1f){
					m_colorfulTime =0f;
					++m_colorfulIndex;
					if(m_colorfulIndex>=paintColorful.Length){
						m_colorfulIndex = 0;
					}
				}
				penMat.color=m_penColor;
			}
			else if(paintType== PaintType.DrawLine){
				penMat.color=m_penColor;
			}
			GL.PushMatrix();
			GL.LoadPixelMatrix(0, rt.width, rt.height, 0);
			RenderTexture.active = rt;
			if(useVectorGraphic){
				VectorGraphicDraw(ref screenPos,ref m_prevMousePosition);
			}else{
				LerpDraw(ref screenPos,ref m_prevMousePosition);
			}
			RenderTexture.active = null;
			GL.PopMatrix();
			m_prevMousePosition = screenPos;

		}
	}

	void LerpDraw(ref Vector3 current ,ref Vector3 prev){
		float distance = Vector2.Distance(current, prev);
		if(distance>0f){
			Vector2 pos;
			float w = penMat.mainTexture.width*brushScale;
			float h = penMat.mainTexture.height*brushScale;
			float lerpDamp = Mathf.Min(w,h)*drawLerpDamp;
			for (float i = 0; i < distance; i += lerpDamp)
			{
				float lDelta = i / distance;
				float lDifx = current.x - prev.x;
				float lDify = current.y - prev.y;
				pos.x = prev.x + (lDifx * lDelta);
				pos.y = prev.y + (lDify * lDelta);
				Rect rect = new Rect(pos.x-w*0.5f,pos.y-h*0.5f,w,h);
				Graphics.DrawTexture(rect,penMat.mainTexture,penMat);
			}
		}
	}

	/// <summary>
	///  矢量绘制，不支持透明通道，PenTex和BurshScale大小只作为画线的宽度
	/// </summary>
	/// <param name="current">Current.</param>
	/// <param name="prev">Previous.</param>
	void VectorGraphicDraw(ref Vector3 current,ref Vector3 prev){
		if(Vector3.Distance(current,prev)>0)
		{
			float radius = penMat.mainTexture!=null ? penMat.mainTexture.width*brushScale*0.5f : brushScale;
			penMat.SetPass(0);
			//draw circle
			float step = 0.2f;
			GL.Begin(GL.TRIANGLE_STRIP);
			GL.TexCoord2(0.5f, 0.5f);
			GL.Color(m_penColor);
			for (float i=-step;i<6.28318f;i+=step)
			{
				GL.Vertex3(prev.x,prev.y,0f);
				GL.Vertex3(prev.x+Mathf.Sin(i)*radius,prev.y+Mathf.Cos(i)*radius,0f);
				GL.Vertex3(prev.x+Mathf.Sin(i+step)*radius,prev.y+Mathf.Cos(i+step)*radius,0f);

				GL.Vertex3(current.x,current.y,0f);
				GL.Vertex3(current.x+Mathf.Sin(i)*radius,current.y+Mathf.Cos(i)*radius,0f);
				GL.Vertex3(current.x+Mathf.Sin(i+step)*radius,current.y+Mathf.Cos(i+step)*radius,0f);
			}
			GL.End();

			//draw rect
			GL.Begin(GL.QUADS);
			GL.TexCoord2(0.5f, 0.5f);
			GL.Color(m_penColor);
			Vector3 dir = (current - prev).normalized;
			Vector3 normal = new Vector2 (-dir.y, dir.x) * radius;
			GL.Vertex (prev + normal);
			GL.Vertex (prev - normal);
			GL.Vertex (current - normal);
			GL.Vertex (current + normal);
			GL.End();
		}
	}

	/// <summary>
	/// click draw texture
	/// </summary>
	/// <param name="screenPos">Screen position.</param>
	/// <param name="camera">Camera is "Camera.main" if value is null</param>
	/// <param name="pen"> User default pen texture if value is null</param>
	public void ClickDraw(Vector3 screenPos , Camera camera=null , Texture pen=null, float penScale=1f , Material drawMat = null , RenderTexture rt=null){
		if (camera == null) camera = Camera.main;
		if(pen==null) pen = penMat.mainTexture;
		if(drawMat==null) drawMat = penMat;
		if(rt==null) rt = m_rt;

		RaycastHit hit;
		if (!Physics.Raycast(camera.ScreenPointToRay(screenPos), out hit))
			return;

		Vector3 uvPos= hit.textureCoord;

		Vector3 scPos = new Vector3(uvPos.x * rt.width, rt.height - uvPos.y * rt.height,0f);
		float w = pen.width*penScale;
		float h = pen.height*penScale;

		Rect rect = new Rect((scPos.x-w*0.5f),(scPos.y-h*0.5f),w,h);

		GL.PushMatrix();
		GL.LoadPixelMatrix(0, rt.width, rt.height, 0);
		RenderTexture.active = rt;
		Graphics.DrawTexture(rect,pen,drawMat);
		RenderTexture.active = null;
		GL.PopMatrix();
	}

	public void DrawRT2OtherRT(RenderTexture rt, RenderTexture otherRt,Material drawMat = null){
		if(drawMat==null) drawMat = penMat;
		if(rt && otherRt){
			GL.PushMatrix();
			GL.LoadPixelMatrix(0, rt.width, rt.height, 0);
			RenderTexture.active = otherRt;
			Graphics.DrawTexture(new Rect(0,0,rt.width,rt.height),rt,drawMat);
			RenderTexture.active = null;
			GL.PopMatrix();
		}
	}

	/// <summary>
	/// draw end
	/// </summary>
	public void EndDraw(){
		m_isDrawing = false;
	}

	/// <summary>
	/// reset canvas
	/// </summary>
	public void ResetCanvas(RenderTexture rt=null)
	{
		if(rt==null) rt = m_rt;
		if(rt){
			Graphics.SetRenderTarget (rt);
			Color c = new Color(0,0,0,0) ;
			if(m_isEraser){
				c.a = 1f;
				GL.Clear(true,true,c);
			}else{
				c.a = 0f;
				if(paintType== PaintType.DrawLine||paintType== PaintType.DrawColorfulLine){
					c = new Color(0,0,0,0);
				}
				GL.Clear(true,true,c);
			}
			RenderTexture.active = null;
		}
	}

	/// <summary>
	/// clear
	/// </summary>
	public void ClearCanvas(RenderTexture rt=null){
		if(rt==null) rt = m_rt;
		if(rt){
			Graphics.SetRenderTarget (rt);
			Color c = new Color(0,0,0,0) ;
			GL.Clear(true,true,c);
			RenderTexture.active = null;
		}
	}

}
