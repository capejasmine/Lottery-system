using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/*
void OnClick(SpriteLayerInputManager.TouchEvent evt){
	print(evt.pointerOnObject.name);
}
void OnClickNone(SpriteLayerInputManager.TouchEvent evt){
	
}
void OnDown( SpriteLayerInputManager.TouchEvent evt){
	transform.DOScale(m_localScale*1.1f,0.1f);
}

void OnUp( SpriteLayerInputManager.TouchEvent evt){
	transform.DOScale(m_localScale,0.1f);
}
*/
/// Sprite touch事件的分发和管理，Sprite上面需要添加Collider2D组件，触发顺序根据z
/// messageReceiver或者SpriteLayerInputManager下面的Sprite会接收到的事件
/// </summary>
public class SpriteLayerInputManager : MonoBehaviour {

	public class TouchEvent{
		public Vector3 pressPosition; //world position
		public Vector3 position; //world position
		public Vector3 detal; //world position detal
		public GameObject pressObject = null ; //按下时的对象
		public GameObject pointerOnObject = null ; //当前鼠标下面的对象
	}

	private TouchEvent m_event ;
	private bool m_isTouchDown = false;
	private float m_touchTimeDelta = 0f;
	private Collider2D m_touchTarget = null ;

	//需不需要判断是否在UI上面
	public bool isCheckOnUGUI = true;
	//哪些层接受touch
	public LayerMask layerMask = -1;
	//计算使用的Camera , 默认为Camera.main
	public Camera raycastCamera;
	//消息的接收者，可以为null，为null则每个触发对象都会收到消息
	public GameObject messageReceiver = null ;
	//多少时间内算点击
	public float clickDelta=0.5f;
	//移动多少距离算点击
	public float clickMoveValid = 10000f;
	//移动多少距离算点击
	public float clickNoneMoveValid = 10000f;

    public event System.Action<TouchEvent> OnClick;
    public event System.Action<TouchEvent> OnUp;
    public event System.Action<TouchEvent> OnDown;
    public event System.Action<TouchEvent> OnClickNone;
    public event System.Action<TouchEvent> OnMove;

	void Start(){
		if(raycastCamera==null){
			raycastCamera = Camera.main;
		}
	}

	void Update()
	{
		if(Input.touchCount<2){
			if(Input.GetMouseButtonDown(0)){
				if(isCheckOnUGUI && InputUtil.CheckMouseOnUI()) return;
				m_isTouchDown = true;
				OnTouchDown();
			}
			else if(m_isTouchDown && Input.GetMouseButtonUp(0)){
				m_isTouchDown = false;
				OnTouchUp();
			}

			if(m_isTouchDown && Input.GetMouseButton(0)){
				OnTouchMove();
			}
		}
	}

	void OnTouchDown(){
		m_touchTimeDelta = Time.realtimeSinceStartup;
		m_event = new TouchEvent();

		m_event.pressPosition = raycastCamera.ScreenToWorldPoint(Input.mousePosition);
		m_event.position = m_event.pressPosition;
		m_touchTarget = Physics2D.OverlapPoint(m_event.pressPosition,layerMask);

		if(m_touchTarget){
			m_event.pressObject = m_touchTarget.gameObject;
            if (OnDown != null)
            {
                OnDown(m_event);
            }
            else
            {
                SendTouchMessage("OnDown");
            }
		}
	}

	void OnTouchUp(){
		m_event.position = raycastCamera.ScreenToWorldPoint( Input.mousePosition);
		m_touchTarget = Physics2D.OverlapPoint(m_event.position,layerMask);
		if(m_touchTarget!=null){
			m_event.pointerOnObject = m_touchTarget.gameObject;
			if(Time.realtimeSinceStartup-m_touchTimeDelta<clickDelta && m_event.pressObject== m_event.pointerOnObject
				&& Vector3.Distance(m_event.pressPosition,m_event.position)<clickMoveValid){
                if (OnClick != null)
                {
                    OnClick(m_event);
                }
                else
                {
                    SendTouchMessage("OnClick");
                }
			}
			
            if (OnUp != null)
            {
                OnUp(m_event);
            }
            else
            {
                SendTouchMessage("OnUp");
            }
		}
		else
		{
			if(m_event.pressObject == null && Time.realtimeSinceStartup-m_touchTimeDelta<clickDelta 
				&& Vector3.Distance(m_event.pressPosition,m_event.position)<clickNoneMoveValid){
                if (OnClickNone != null)
                {
                    OnClickNone(m_event);
                }
                else
                {
                    SendTouchMessage("OnClickNone");
                }
			}
			m_event.pointerOnObject = null;
            if (OnUp != null)
            {
                OnUp(m_event);
            }
            else
            {
                SendTouchMessage("OnUp");
            }
		}
		m_event = null;
		m_touchTarget = null;
		m_touchTimeDelta = 0f;
	}

	void OnTouchMove(){
		Vector3 pos = raycastCamera.ScreenToWorldPoint(Input.mousePosition);
		m_event.detal = pos-m_event.position;
		m_event.position = pos;
		m_touchTarget = Physics2D.OverlapPoint(m_event.position,layerMask);
		if(m_touchTarget!=null){
			m_event.pointerOnObject = m_touchTarget.gameObject;
            if (OnMove != null)
            {
                OnMove(m_event);
            }
            else
            {
                SendTouchMessage("OnMove");
            }
		}
		else
		{
			m_event.pointerOnObject = null;
            if (OnMove != null)
            {
                OnMove(m_event);
            }
            else
            {
                SendTouchMessage("OnMove");
            }
		}
	}

	void SendTouchMessage(string method){
		if(messageReceiver==null){
			if(m_event.pointerOnObject)
			{
				m_event.pointerOnObject.SendMessage(method,m_event,SendMessageOptions.DontRequireReceiver);
			}
			if(m_event.pressObject && m_event.pointerOnObject!=m_event.pressObject){
				m_event.pressObject.SendMessage(method,m_event,SendMessageOptions.DontRequireReceiver);
			}
		}else{
			messageReceiver.SendMessage(method,m_event,SendMessageOptions.DontRequireReceiver);
		}
	}
}
