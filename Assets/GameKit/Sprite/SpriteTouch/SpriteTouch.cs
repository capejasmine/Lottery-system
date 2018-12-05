using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// 单一一个Sprite的touch管理
/// </summary>
public class SpriteTouch : MonoBehaviour {

	public LayerMask layerMask;
	public bool checkOnUGUI = true;
	public bool supportMultiTouch = false;

	private Collider2D m_col;
	private bool m_isDown = false;
	private float m_touchTimeDelta;
	private int m_fingerId = -1;

	[SerializeField]
	protected UnityEvent m_onClick = new UnityEvent();
	public UnityEvent onClick{
		get{return m_onClick; }
	}
	[SerializeField]
	protected UnityEvent m_onDown = new UnityEvent();
	public UnityEvent onDown{
		get{return m_onDown; }
	}
	[SerializeField]
	protected UnityEvent m_onUp = new UnityEvent();
	public UnityEvent onUp{
		get{return m_onUp; }
	}

	// Use this for initialization
	void Start () {
		m_col = GetComponent<Collider2D>();
	}

	// Update is called once per frame
	void Update () {
		if(m_col==null || m_col.enabled==false) return;

		if(supportMultiTouch && (m_fingerId>-1 || Input.touchCount>0))
		{

			foreach(Touch touch in Input.touches)
			{
				if(touch.phase == TouchPhase.Began && (!checkOnUGUI || !InputUtil.CheckMouseOnUI()) )
				{
					Collider2D col=Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(touch.position),layerMask);
					if(col==m_col ){
						m_fingerId = touch.fingerId;
						m_touchTimeDelta = Time.realtimeSinceStartup;
						m_onDown.Invoke();
						break;
					}
				}
			}

			if(m_fingerId>-1 ){
				foreach(Touch touch in Input.touches)
				{
					if(touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
					{
						Collider2D col=Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(touch.position),layerMask);
						if(touch.fingerId==m_fingerId ){
							m_fingerId = -1;
							if(col==m_col && Time.realtimeSinceStartup-m_touchTimeDelta<0.5f){
								m_onClick.Invoke();
							}
							m_onUp.Invoke();
						}
					}
				}
			}

		}
		else
		{

			if(Input.GetMouseButtonDown(0) && (!checkOnUGUI || !InputUtil.CheckMouseOnUI()) ){
				Collider2D[] cols=Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition),layerMask);
				if(cols !=null && cols.Length>0 && cols[0]==m_col ){
					m_isDown = true;
					m_touchTimeDelta = Time.realtimeSinceStartup;
					m_onDown.Invoke();
				}
			}

			if(m_isDown && Input.GetMouseButtonUp(0)){
				if(Time.realtimeSinceStartup-m_touchTimeDelta<0.5f){
					Collider2D[] cols=Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition),layerMask);
					if(cols !=null && cols.Length>0 && cols[0]==m_col ){
						m_onClick.Invoke();
					}
				}
				m_isDown = false;
				m_onUp.Invoke();
			}

		}
	}
}
