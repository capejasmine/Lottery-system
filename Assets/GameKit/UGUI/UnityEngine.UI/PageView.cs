using UnityEngine;
using System;
using System.Collections;
//using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	class PageItem:MonoBehaviour{
		protected internal int index = 0;
	}

    /// <summary>
    /// author:zhouzhanglin
    /// 分页显示.
    /// </summary>
    [AddComponentMenu("UI/Page View",1350)]
    [RequireComponent(typeof(RectTransform))]
	public class PageView : ScrollRect, IPointerUpHandler,IPointerExitHandler,IPage
    {
		private float m_dragTime = 0 ;
        private bool m_drag = false;
        private Vector2 m_end = Vector2.zero;

        private int m_totalPage = 0;
        //从0开始.
        public int totalPage { get {return  m_totalPage;  } }

        private int m_currentPage = 0;
        //从0开始.
		public int currentPage { get { return content.GetChild(m_currentPage).GetComponent<PageItem>().index; } }

        private bool _enableUpdate = false;

        private Vector2 m_contentPos = Vector2.zero;
		private PointerEventData m_dragEventData;

		public PageIndicator pageIndicator;
		//缓动.
		public float pageDamp = 0.2f;
		//在区域外时是否还可以拖动
		public bool dragEnableOutSide = true;
		//是否自动初始化.
		public bool autoInit = true;
		public bool isLoop = false;
		//事件
		public UnityEvent onScrollOver;
		public UnityEvent onPageChange ;

		public Action<Vector2> onBeginDragAction;
		public Action<Vector2> onDragAction;
		public Action<Vector2> onEndDragAction;

		/// <summary>
		/// 用于点击PageIndicator时改变页码.
		/// </summary>
		/// <param name="index"></param>
		public void ShowPage(int index)
		{
			GotoPage(index);
		}
		/// <summary>
		/// 用于PageIndicator
		/// </summary>
		/// <returns><c>true</c>, if is loop was paged, <c>false</c> otherwise.</returns>
		public bool PageIsLoop(){
			return isLoop;
		}

		public override void Rebuild(CanvasUpdate executing)
		{
			if (executing != CanvasUpdate.PostLayout)
				return;
			base.Rebuild(executing);
	
			if(Application.isPlaying && autoInit){
				Init();
			}
		}

        /// <summary>
        /// 初始化此控件.
        /// </summary>
		public PageView Init(int page = 0)
        {
			
            if (horizontal)
            {
                content.pivot = new Vector2(0, 0.5f); //内容的原点在最左边.
            }
            else if (vertical)
            {
                content.pivot = new Vector2(0.5f, 1f); //内容的原点在最上边.
            }
            m_totalPage = content.childCount - 1;


//			Vector2 itemPos = Vector2.one;
//			Vector2 itemSize = Vector2.one;
//			Vector2 itemAnchorMin = Vector2.one;
//			Vector2 itemAnchorMax = Vector2.one;
//			Vector2 itemPivot = Vector2.one;
//			Vector2 itemOffsetMin = Vector2.one;
//			Vector2 itemOffsetMax = Vector2.one;
//			List<Vector2> pos = new List<Vector2>();
//
//			if (content.childCount > 0) {
//				RectTransform child = content.GetChild (0).transform as RectTransform;
//				itemSize = child.sizeDelta;
//				itemPos = child.anchoredPosition;
//				itemAnchorMin = child.anchorMin;
//				itemAnchorMax = child.anchorMax;
//				itemPivot = child.pivot;
//				itemOffsetMin = child.offsetMin;
//				itemOffsetMax = child.offsetMax;
//				foreach (RectTransform ch in content)
//				{
//					pos.Add(ch.anchoredPosition);
//				}
//			}

			LayoutGroup group = transform.GetComponentInChildren<LayoutGroup>();
			if(group && Application.isPlaying){
				group.enabled = false;
			}

			int i = 0;
			foreach(RectTransform child in content){
//				itemPivot = child.pivot;
//				child.offsetMin = itemOffsetMin;
//				child.offsetMax = itemOffsetMax;
//				child.anchorMin = itemAnchorMin;
//				child.anchorMax = itemAnchorMax;
//				child.sizeDelta = itemSize;
//				child.anchoredPosition = pos[i];

				PageItem item = child.GetComponent<PageItem>();
				if (item == null)
				{
					item = child.gameObject.AddComponent<PageItem>();
				}
				item.index = i;

//				LayoutGroup childGroup = item.GetComponent<LayoutGroup>();
//				if(childGroup && childGroup.enabled){
//					childGroup.enabled = false;
//					childGroup.enabled = true;
//				}
				++i;
			}


            m_end = -((RectTransform)content.GetChild(0).transform).anchoredPosition;
            SetContentAnchoredPosition(m_end);

			if (pageIndicator)
			{
				pageIndicator.iPage = this;
				pageIndicator.Build(totalPage + 1);
			}
            return this;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 1) return;

			Graphic g = GetComponent<Graphic>();
			if (g==null || g.raycastTarget==false) return;

            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            m_drag = true;
            _enableUpdate = true;
			m_isIn = true;
            //记录开始拖动时内容的位置和开始拖动时的时间戳.
            m_contentPos = content.anchoredPosition;
			m_dragTime = Time.realtimeSinceStartup;
            base.OnBeginDrag(eventData);
			if(onBeginDragAction!=null) onBeginDragAction(eventData.delta);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 1) return;
			if(!dragEnableOutSide && !m_isIn){
				return;
			}
			m_dragEventData = eventData;

			Graphic g = GetComponent<Graphic>();
			if (g && g.raycastTarget)
                 base.OnDrag(eventData);

			if(isLoop){
				CheckLoop(eventData);
			}
			if(onDragAction!=null) onDragAction(eventData.delta);
        }

		void OnApplicationFocus(bool flag){
			if(!flag){
				m_isIn = false;
				if(m_dragEventData!=null)
                    OnEndDrag(m_dragEventData);
			}
		}

		void CheckLoop(PointerEventData eventData){
			if(horizontal){
				if(eventData.delta.x>0){
					RectTransform child = content.GetChild(0) as RectTransform;
					if(child.anchoredPosition.x + content.anchoredPosition.x > -child.sizeDelta.x ){
						RectTransform lastChild = content.GetChild(content.childCount-1) as RectTransform;
						Vector2 v = lastChild.anchoredPosition;
						v.x = child.anchoredPosition.x - viewRect.sizeDelta.x;
						lastChild.anchoredPosition = v;
						lastChild.SetAsFirstSibling();
					}
				}else{
					RectTransform child = content.GetChild(content.childCount-1) as RectTransform;
					if(child.anchoredPosition.x + content.anchoredPosition.x< child.sizeDelta.x){
						RectTransform firstChild = content.GetChild(0) as RectTransform;
						Vector2 v = firstChild.anchoredPosition;
						v.x = child.anchoredPosition.x + viewRect.sizeDelta.x;
						firstChild.anchoredPosition = v;
						firstChild.SetAsLastSibling();
					}
				}
			}
			else if(vertical)
			{
				if(eventData.delta.y<0){
					RectTransform child = content.GetChild(0) as RectTransform;
					if(child.anchoredPosition.y + content.anchoredPosition.y< child.sizeDelta.y ){
						RectTransform lastChild = content.GetChild(content.childCount-1) as RectTransform;
						Vector2 v = lastChild.anchoredPosition;
						v.y = child.anchoredPosition.y + viewRect.sizeDelta.y;
						lastChild.anchoredPosition = v;
						lastChild.SetAsFirstSibling();
					}
				}else{
					RectTransform child = content.GetChild(content.childCount-1) as RectTransform;
					if(child.anchoredPosition.y + content.anchoredPosition.y> -child.sizeDelta.y){
						RectTransform firstChild = content.GetChild(0) as RectTransform;
						Vector2 v = firstChild.anchoredPosition;
						v.y = child.anchoredPosition.y - viewRect.sizeDelta.y;
						firstChild.anchoredPosition = v;
						firstChild.SetAsLastSibling();
					}
				}
			}
		}

        override public void OnEndDrag(PointerEventData eventData)
        {
			Graphic g = GetComponent<Graphic>();
			if (g==null || g.raycastTarget==false) return;
            
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			if(!m_drag) return;
            m_drag = false;
            base.OnEndDrag(eventData);

			float dis = horizontal? Mathf.Abs(eventData.pressPosition.x-eventData.position.x) : Mathf.Abs(eventData.pressPosition.y-eventData.position.y);
			if (Time.realtimeSinceStartup - m_dragTime < 0.2f && dis>50f)
            {
                //拖动的时间比较快时换页码.
                if(horizontal){
					if (eventData.position.x-eventData.pressPosition.x>= 0.5f)
                    {
                        PrevPage();
                    }
					else if (eventData.position.x -eventData.pressPosition.x<= -0.5f)
                     {
                        NextPage();
                    }
                }
                else if (vertical)
                {
					if (eventData.position.y -eventData.pressPosition.y>= 0.5f)
                    {
                        NextPage();
                    }
					else if (eventData.position.y-eventData.pressPosition.y <= -0.5f)
                    {
                        PrevPage();
                    }
                }
            }
            else
            {
                //拖动的距离超过一半时换页码.
                if (horizontal)
                {
                    if (content.anchoredPosition.x - m_contentPos.x > viewRect.sizeDelta.x / 2f)
                    {
                        PrevPage();
                    }
					else if (content.anchoredPosition.x - m_contentPos.x < -viewRect.sizeDelta.x / 2f)
                    {
                        NextPage();
                    }
                }
                else if (vertical)
                {
					if (content.anchoredPosition.y - m_contentPos.y > viewRect.sizeDelta.y / 2f)
                    {
                        NextPage();
                    }
					else if (content.anchoredPosition.y - m_contentPos.y < -viewRect.sizeDelta.y / 2f)
                    {
                        PrevPage();
                    }
                }
			}
			if(onEndDragAction!=null) onEndDragAction(eventData.delta);
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            OnEndDrag(eventData);
        }

        /// <summary>
        /// 鼠标中键滑动翻页，暂时没有实现.
        /// </summary>
        /// <param name="data"></param>
        override public void OnScroll(PointerEventData data)
        {

        }

        override protected void LateUpdate()
        {
            if (m_drag)
            {
                base.LateUpdate();
            }
			else if(_enableUpdate)
            {
                if (horizontal )
                {
                    if (Mathf.Abs(content.anchoredPosition.x - m_end.x) > 1f)
                    {
                        Vector2 pos = Vector2.Lerp(content.anchoredPosition, m_end, pageDamp);
                        SetContentAnchoredPosition(pos);
                    }
                    else
                    {
                        SetContentAnchoredPosition(m_end);
                        _enableUpdate = false;
                        onScrollOver.Invoke();
                    }
                }
                else if (vertical)
                {
                    if (Mathf.Abs(content.anchoredPosition.y - m_end.y) > 1f)
                    {
                        Vector2 pos = Vector2.Lerp(content.anchoredPosition, m_end, pageDamp);
                        SetContentAnchoredPosition(pos);
                    }
                    else
                    {
                        SetContentAnchoredPosition(m_end);
                        _enableUpdate = false;
                        onScrollOver.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// 是否还有下一页.
        /// </summary>
        /// <returns></returns>
        public bool HasNextPage()
		{
			if(isLoop && m_totalPage > 1 ) return true;
            return m_currentPage < m_totalPage;
        }
        /// <summary>
        /// 是否有上一页.
        /// </summary>
        /// <returns></returns>
        public bool HasPrevPage()
        {
			if(isLoop && m_totalPage > 1 ) return true;
            return m_currentPage > 0;
        }

        /// <summary>
        /// 下一页.
        /// </summary>
        public void NextPage()
        {
            if (HasNextPage())
            {
                GotoPage(m_currentPage+1);
            }
        }
        /// <summary>
        /// 上一页.
        /// </summary>
        public void PrevPage()
        {
            if (HasPrevPage())
            {
                GotoPage(m_currentPage-1);
            }
        }

        /// <summary>
        /// 切换到哪一页.
        /// </summary>
        /// <param name="pageIndex">从0开始.</param>
        /// <param name="anim">是否显示翻页动画.默认为true.</param>
        public void GotoPage(int pageIndex,bool anim=true)
		{
			m_drag = false;
            if (m_currentPage != pageIndex)
			{
				if(isLoop){
					if(pageIndex<0) pageIndex = m_totalPage;
					else if(pageIndex>m_totalPage) pageIndex = 0;
				}else{
					if(pageIndex<0) pageIndex = 0;
					else if(pageIndex>m_totalPage) pageIndex = m_totalPage;
				}

				m_currentPage = pageIndex;

				RectTransform child = null;
				foreach(Transform temp in content){
					PageItem item = temp.gameObject.GetComponent<PageItem>();
					if(item.index == pageIndex){
						child = temp as RectTransform;
						break;
					}
				}
				if(child==null) return;

				Vector2 endPos = -child.anchoredPosition;
                m_end = endPos;
                _enableUpdate = true;
                onPageChange.Invoke();
                if (pageIndicator)
                {
					pageIndicator.ShowPage(child.GetComponent<PageItem>().index);
                }

                if (!anim)
                {
                    _enableUpdate = false;
                    SetContentAnchoredPosition(m_end);
                }
            }
        }

		#region 是否在区域内
		private bool m_isIn = false;
		public void OnPointerExit (PointerEventData eventData)
		{
			m_isIn = false;
		}
		#endregion
    }
}


