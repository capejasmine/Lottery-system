using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

/// <summary>
/// Drag and drop sprite.需要dragTarget上面有Collider2D组件
/// Author :zhouzhanglin
/// </summary>
public class SpriteDrag : MonoBehaviour {

	public enum DragBackEffect{
		None,Immediately, TweenPosition, TweenScale , ScaleDestroy , FadeOutDestroy , Destroy, Keep
	}
	public enum TriggerType{
		Point,Circle,Range
	}

	private float m_dragMoveDamp = 0f;
	private Vector3 m_cachePosition; //全局坐标
	private Vector3 m_cacheScale;
	private Vector3 m_cacheRotation;
	private Vector3 m_defaultPosition; //全局坐标
	private Vector3 m_defaultScale;
	private Vector3 m_defaultRotation;
	private Vector3 m_dragOffset;
	private Vector3 m_screenPosition;
	private Vector3 m_currentPosition;
	private Vector3 m_mousePressPosition; //moues按下时的位置
	private float m_downTime; //mouse按下时的Time.realtimeSinceStartup 时间 
	private bool m_isDown;
	private bool m_isDragging = false;
	private bool m_canDrag = true;
	private string m_sortLayerName;

	#region getter/setter
	public float mouseDownTime{
		get { return m_downTime; }
	}
	public Vector3 mousePressPosition{
		get { return m_mousePressPosition; }
	}
	public bool isDragging{
		get { return m_isDragging && m_isDown; }
	}
	public bool canDrag{
		get { return m_canDrag; }
		set { 
			m_canDrag = value; 
			if(!value) {
				m_isDragging = false;
				m_isDown = false;
			}

		}
	}
	//world
	public Vector3 orginToTriggerOffset{
		get{ return dragTarget.position-triggerPos.position; }
	}
	#endregion

	[Tooltip("拖动的对象，默认为自己.")]
	public Transform dragTarget = null;

	[Tooltip("如果为null，则使用mainCamera.")]
	public Camera rayCastCamera = null;

	[Tooltip("射线检测的Layer")]
	public LayerMask dragRayCastMask=-1;
	public LayerMask dropRayCastMask=-1;
	[Tooltip("射线检测的深度(-raycastDepth,raycastDepth)")]
	public float raycastDepth = 100f;
	public bool dragCheckUGUI = true;//drag时是否判断在ugui上

	[Tooltip("判断drag时是否忽略上面")]
	public bool dragIgnoreTop = true;
	[Tooltip("判断drop时是否忽略下面")]
	public bool dropIgnoreBottom = true;

	[Header("Drag Setting")]
	[Tooltip("在拖动时是否固定在拖动物的原点.")]
	public bool isDragOriginPoint = false;

	[Tooltip("当isDragOriginPoint为true时，拖动时的偏移值.")]
	public Vector2 dragOffset;

	[Tooltip("主要用于影响层级显示.")]
	public float dragOffsetZ=0f;

	[Tooltip("Drag时的变化的大小.")]
	public float dragChangeScale = 1f;

	[Tooltip("Drag时角度的变化值")]
	public float dragChangeRotate = 0f;

	[Tooltip("拖动的时候在哪个层.没有设置的话为当前Sort Layer")]
	public string dragSortLayerName;

	[Tooltip("拖动时变化的层级数")]
	public int dragChangeOrder = 0;

	[Tooltip("触发的原点，默认为当前对象")]
	public Transform triggerPos ;

	[Tooltip("触发的类型")]
	public TriggerType triggerType=TriggerType.Point;

	[Tooltip("当触发类型为圆时,设置半径")]
	public float triggerRadius=0.5f;

	[Tooltip("当触发类型为范围时,设置宽高")]
	public Vector2 triggerRange = new Vector2(0.5f,0.5f);

	//要发送的事件名字
	[Header("Event")]
	public bool sendHoverEvent = false;
	public string onHoverMethodName = "OnHover";
	public string onHoverOutMethodName = "OnHoverOut";
	public string onDropMethodName = "OnDrop";

	[Header("Back Effect")]
	[Tooltip("释放时，是否自动返回")]
	public bool releaseAutoBack = false;
	[Tooltip("返回时的效果")]
	public DragBackEffect backEffect = DragBackEffect.None;
	[Tooltip("效果时间")]
	public float backDuring = 0.5f;
	[Tooltip("Tween 的效果")]
	public Ease tweenEase = Ease.Linear;

	public event Action<SpriteDrag> OnPrevBeginDragAction = null ;
	public event Action<SpriteDrag> OnBeginDragAction = null ;
	public event Action<SpriteDrag> OnDragAction = null ;
	public event Action<SpriteDrag> OnEndDragAction = null ;
	public event Action<SpriteDrag> OnPrevEndDragAction = null ;
	public event Action<SpriteDrag> OnTweenStartAction=null,OnTweenBackAction = null ;
	public delegate bool DragValidCheck();
	public event DragValidCheck DragValidCheckEvent;

	public event Action<SpriteDrag,Collider2D[]>  OnHoverColliderAction;
	public event Action<SpriteDrag,Collider2D[]>  OnDropColliderAction;
	public event Action<SpriteDrag>  OnHoverColliderOutAction;

	void OnEnable(){
		m_isDown = false;
		m_isDragging = false;
	}

	void OnDisable(){
		m_isDown = false;
		m_isDragging = false;
	}

	// Use this for initialization
	void Start () {
		if (!dragTarget){
			dragTarget = transform;
		}
		if(!triggerPos){
			triggerPos = dragTarget;
		}
		if (!rayCastCamera)
		{
			rayCastCamera = Camera.main;
		}
		m_defaultScale = dragTarget.localScale;
		m_defaultRotation = dragTarget.localEulerAngles;
		m_defaultPosition = dragTarget.position;
		SpriteRenderer spriteRender = dragTarget.GetComponentInChildren<SpriteRenderer>();
		m_sortLayerName = spriteRender.sortingLayerName;
		if(string.IsNullOrEmpty(dragSortLayerName)){
			dragSortLayerName = m_sortLayerName;
		}
	}

	public void SetDefaultPosition(){
		if(dragTarget) dragTarget.position = m_defaultPosition;
	}
	public void SetDefaultRotation(){
		if(dragTarget) dragTarget.localEulerAngles = m_defaultRotation;
	}
	public void SetDefaultScale(){
		if(dragTarget) dragTarget.localScale = m_defaultScale;
	}
	
	// Update is called once per frame
	void Update () {
		if(!this.isActiveAndEnabled) return;
		if (Input.touchCount < 2)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (!m_isDown && !InputUtil.isOnUI)
				{
					if(dragCheckUGUI && InputUtil.CheckMouseOnUI()) return;

					RaycastHit2D[] hits = Physics2D.RaycastAll(rayCastCamera.ScreenToWorldPoint(Input.mousePosition),Vector2.zero, 0, dragRayCastMask);
					if(hits!=null && hits.Length>0){
						foreach(RaycastHit2D hit in hits){
							if (hit.collider.gameObject == gameObject)
							{
								m_isDown = true;
								m_downTime = Time.realtimeSinceStartup;
								m_mousePressPosition = Input.mousePosition;
								break;
							}
							if(dragIgnoreTop) break;
						}
					}
				}
			}
			else if (m_isDown && Input.GetMouseButton(0))
			{
				OnPointerDragHandler();
			}
			else if (m_isDown && Input.GetMouseButtonUp(0))
			{
				OnPointerUpHandler();
			}
		}
	}

	void OnPointerBeginDrag(){
		if(!this.enabled || !m_isDown) return;

		if(DragValidCheckEvent!=null) {
			if(!DragValidCheckEvent()){
				m_canDrag = false;
				return;
			}
		}
		if(OnPrevBeginDragAction!=null){
			OnPrevBeginDragAction(this);
		}

		m_dragMoveDamp = 0.3f;
		this.m_canDrag = true;
		this.m_isDragging = true;
		dragTarget.DOKill();

		m_cachePosition = dragTarget.position;
		m_cacheScale = dragTarget.localScale;
		m_cacheRotation = dragTarget.localEulerAngles;
		if(dragChangeScale!=1f){
			dragTarget.DOScale(m_cacheScale*dragChangeScale,0.25f);
		}
		if(dragChangeRotate!=0f){
			dragTarget.DOLocalRotate(m_cacheRotation +new Vector3(0f,0f,dragChangeRotate),0.4f,RotateMode.Fast);
		}

		dragTarget.position+=new Vector3(0,0,dragOffsetZ);
		m_currentPosition = m_cachePosition;
		m_dragOffset = Vector3.zero;

		foreach(SpriteRenderer render in dragTarget.GetComponentsInChildren<SpriteRenderer>()){
			render.sortingLayerName=dragSortLayerName;
			render.sortingOrder += dragChangeOrder;
		}

		m_screenPosition = rayCastCamera.WorldToScreenPoint(dragTarget.position);
		if (!isDragOriginPoint)
		{
			m_dragOffset = dragTarget.position - rayCastCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_screenPosition.z));
		}
		if (OnBeginDragAction!=null)
		{
			OnBeginDragAction(this);
		}
	}

	void OnPointerDragHandler(){
		if(this.enabled && !m_isDragging){
			OnPointerBeginDrag();
		}

		if(!this.enabled  || !m_isDragging)  return;
		if(m_dragMoveDamp<1f) m_dragMoveDamp+=0.01f;

		m_screenPosition = rayCastCamera.WorldToScreenPoint(dragTarget.position);
		Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_screenPosition.z);
		m_currentPosition = rayCastCamera.ScreenToWorldPoint(curScreenSpace);
		if (!isDragOriginPoint){
			m_currentPosition += m_dragOffset;
		}else{
			m_currentPosition += (Vector3)dragOffset;
		}
		dragTarget.position = Vector3.Lerp(dragTarget.position, m_currentPosition, m_dragMoveDamp);
		if(sendHoverEvent){
			Collider2D[] cols = null;
			if(triggerType== TriggerType.Point){
				cols = Physics2D.OverlapPointAll(triggerPos.position,dropRayCastMask,-raycastDepth,raycastDepth);
			}else if(triggerType== TriggerType.Circle){
				cols = Physics2D.OverlapCircleAll(triggerPos.position,triggerRadius,dropRayCastMask,-raycastDepth,raycastDepth);
			}else if(triggerType== TriggerType.Range){
				cols = Physics2D.OverlapBoxAll (triggerPos.position, triggerRange*2f, triggerPos.eulerAngles.z,dropRayCastMask,-raycastDepth,raycastDepth);
			}
			if(cols!=null && cols.Length>0){
				if(OnHoverColliderAction!=null) {
					OnHoverColliderAction(this,cols);
				}else{
					foreach(Collider2D col in cols){
						if(col.gameObject!=gameObject)
							col.SendMessage(onHoverMethodName, dragTarget.gameObject , SendMessageOptions.DontRequireReceiver);
						if(dropIgnoreBottom) break;
					}
					gameObject.SendMessage(onHoverMethodName, cols , SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				if(OnHoverColliderOutAction!=null) {
					OnHoverColliderOutAction(this);
				}else{
					gameObject.SendMessage(onHoverOutMethodName,SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		if (OnDragAction!=null)
		{
			OnDragAction(this);
		}
	}

	void OnApplicationFocus(bool flag){
		if(!flag && m_canDrag && m_isDragging){
			OnPointerUpHandler();
		}
	}

	void OnPointerUpHandler(){
		m_isDown = false;

		if(!this.enabled || !m_isDragging) return;
		m_isDragging = false;

		if(OnPrevEndDragAction!=null){
			OnPrevEndDragAction.Invoke(this);
		}

		DOTween.Kill(dragTarget);
		if(dragChangeScale!=0f){
			dragTarget.DOScale(m_cacheScale,0.25f);
		}
		if(dragChangeRotate!=0f){
			dragTarget.DOLocalRotate(m_cacheRotation,0.25f,RotateMode.Fast);
		}


		if(releaseAutoBack){
			BackPosition();
		}else{
			dragTarget.position -=new Vector3(0,0,dragOffsetZ);
			foreach(SpriteRenderer render in dragTarget.GetComponentsInChildren<SpriteRenderer>()){
				render.sortingLayerName=m_sortLayerName;
				render.sortingOrder -= dragChangeOrder;
			}
		}

		if(sendHoverEvent){
			Collider2D[] cols = null;
			if(triggerType== TriggerType.Point){
				cols = Physics2D.OverlapPointAll(triggerPos.position,dropRayCastMask,-raycastDepth,raycastDepth);
			}else if(triggerType== TriggerType.Circle){
				cols = Physics2D.OverlapCircleAll(triggerPos.position,triggerRadius,dropRayCastMask,-raycastDepth,raycastDepth);
			}else if(triggerType== TriggerType.Range){
				cols = Physics2D.OverlapBoxAll (triggerPos.position, triggerRange*2f, triggerPos.eulerAngles.z,dropRayCastMask,-raycastDepth,raycastDepth);
			}
			if(cols != null && cols.Length>0){
				if(OnDropColliderAction!=null) {
					OnDropColliderAction(this,cols);
				}else{
					foreach(Collider2D col in cols){
						if(col.gameObject!=gameObject)
							col.SendMessage(onDropMethodName, dragTarget.gameObject , SendMessageOptions.DontRequireReceiver);
						if(dropIgnoreBottom) break;
					}
					gameObject.SendMessage(onDropMethodName, cols , SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		if (OnEndDragAction!=null)
		{
			OnEndDragAction(this);
		}
	}

	public void BackPosition(DragBackEffect effect){
		DragBackEffect cache = backEffect;
		backEffect = effect ;
		BackPosition();
		backEffect = cache;
	}

	/// <summary>
	/// 返回原来位置
	/// </summary>
	public void BackPosition(){
		if(backEffect== DragBackEffect.Keep || dragTarget == null) return;

		if(OnTweenStartAction!=null){
			OnTweenStartAction(this);
		}
		switch(backEffect)
		{
		case DragBackEffect.Immediately:
			foreach(SpriteRenderer render in dragTarget.GetComponentsInChildren<SpriteRenderer>()){
				render.sortingLayerName=m_sortLayerName;
				render.sortingOrder -= dragChangeOrder;
			}
			dragTarget.position=m_cachePosition;
			dragTarget.localEulerAngles=m_cacheRotation;
			dragTarget.localScale=m_cacheScale;
			break;
		case DragBackEffect.Destroy:
			Destroy(dragTarget.gameObject);
			break;
		case DragBackEffect.TweenPosition:
			this.enabled = false;
			this.m_canDrag = false;
			dragTarget.DOLocalRotate(m_cacheRotation,backDuring).SetEase(tweenEase);
			dragTarget.DOScale(m_cacheScale,backDuring).SetEase(tweenEase);
			dragTarget.DOMove(new Vector3(m_cachePosition.x,m_cachePosition.y,dragTarget.position.z),backDuring).SetEase(tweenEase).OnComplete(()=>{
				this.enabled = true;
				this.m_canDrag = true;
				dragTarget.position=m_cachePosition;
				foreach(SpriteRenderer render in dragTarget.GetComponentsInChildren<SpriteRenderer>()){
					render.sortingLayerName=m_sortLayerName;
					render.sortingOrder -= dragChangeOrder;
				}
				if(OnTweenBackAction!=null){
					OnTweenBackAction(this);
				}
			});
			break;
		case DragBackEffect.TweenScale:
			this.enabled = false;
			this.m_canDrag = false;
			foreach(SpriteRenderer render in dragTarget.GetComponentsInChildren<SpriteRenderer>()){
				render.sortingLayerName=m_sortLayerName;
				render.sortingOrder -= dragChangeOrder;
			}
			dragTarget.position=m_cachePosition;
			dragTarget.localScale = Vector3.zero;
			dragTarget.localEulerAngles=m_cacheRotation;
			dragTarget.DOScale(m_cacheScale,backDuring).SetEase(tweenEase).OnComplete(()=>{
				this.enabled = true;
				this.m_canDrag = true;
				if(OnTweenBackAction!=null){
					OnTweenBackAction(this);
				}
			});
			break;
		case DragBackEffect.ScaleDestroy:
			this.enabled = false;
			this.m_canDrag = false;
			dragTarget.DOScale(Vector3.zero,backDuring).SetEase(tweenEase).OnComplete(()=>{
				Destroy(dragTarget.gameObject);
				if(OnTweenBackAction!=null){
					OnTweenBackAction(this);
				}
			});
			break;
		case DragBackEffect.FadeOutDestroy:
			this.enabled = false;
			this.m_canDrag = false;
			foreach(SpriteRenderer render in dragTarget.GetComponentsInChildren<SpriteRenderer>()){
				render.DOFade(0,backDuring);
			}
			Destroy(dragTarget.gameObject,backDuring+.1f);
			break;
		default:
			foreach(SpriteRenderer render in dragTarget.GetComponentsInChildren<SpriteRenderer>()){
				render.sortingLayerName=m_sortLayerName;
				render.sortingOrder -= dragChangeOrder;
			}
			dragTarget.position=m_cachePosition;
			dragTarget.localEulerAngles=m_cacheRotation;
			dragTarget.localScale=m_cacheScale;
			break;
		}
	}


	#if UNITY_EDITOR
	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Transform origin = triggerPos == null ?  (dragTarget==null ? transform : dragTarget) : triggerPos;
		if(triggerType == TriggerType.Point)
		{
			Gizmos.DrawSphere(origin.position,0.05f);
		}
		else if(triggerType == TriggerType.Circle)
		{
			Gizmos.DrawWireSphere(origin.position,triggerRadius);
		}
		else if(triggerType== TriggerType.Range)
		{
			Matrix4x4 mat = Gizmos.matrix;
			Gizmos.matrix = Matrix4x4.TRS(origin.position,origin.rotation,Vector3.one);
			Gizmos.DrawWireCube(Vector3.zero,(Vector3)triggerRange*2f);
			Gizmos.matrix = mat;
		}
	}
	#endif 
}
