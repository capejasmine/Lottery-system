﻿using UnityEngine;
using System;
using System.Collections;
//using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    /// <summary>
    /// author:zhouzhanglin
    /// 居中显示.
    /// 说明：如果要调用此类中的属性和方法.
    /// </summary>
    [AddComponentMenu("UI/Center View", 1351)]
    [RequireComponent(typeof(RectTransform))]
	public class CenterView : ScrollRect, IPointerUpHandler,IPage
    {
      
        private bool m_drag = false;
        private Vector2 m_end = Vector2.zero;

		private float m_dragTime = 0;
        private int m_totalPage = 0;
        //从0开始.
        public int totalPage { get { return m_totalPage; } }

        private int m_currentPage = 0;
        //从0开始.
        public int currentPage { get { return m_currentPage; } }

        private bool _enableUpdate = false;

        private Vector2 m_contentPos = Vector2.zero;
        private ArrayList m_all = new ArrayList(); //用于排序.

        //当前居中显示的是哪个item
        private RectTransform m_centerItem=null;
        public RectTransform CenterItem
        {
            get { return m_centerItem; }
        }




		public PageIndicator pageIndicator;
		//缓动.
		public float pageDamp = 0.2f;

		public float maxScale = 1f;
		public float minScale = 0.2f;

		//是否点击后显示在中间.
		public bool clickItemToCenter = false;

		//是否自动初始化 .
		public bool autoInit = true;

		public UnityEvent onScrollOver;
		public UnityEvent onPageChange;
		public UnityEvent onSelect;

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
			return false;
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
        public CenterView Init()
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
			foreach (RectTransform child in content)
			{
//				itemPivot = child.pivot;
//				child.offsetMin = itemOffsetMin;
//				child.offsetMax = itemOffsetMax;
//				child.anchorMin = itemAnchorMin;
//				child.anchorMax = itemAnchorMax;
//				child.sizeDelta = itemSize;
//				child.anchoredPosition = pos[i];

				if(m_centerItem==null) m_centerItem = child;

				CenterViewItem item = child.GetComponent<CenterViewItem>();
				if (item == null)
				{
					item = child.gameObject.AddComponent<CenterViewItem>();
				}
//				LayoutGroup childGroup = item.GetComponent<LayoutGroup>();
//				if(childGroup && childGroup.enabled){
//					childGroup.enabled = false;
//					childGroup.enabled = true;
//				}
				item.index = child.GetSiblingIndex();
				item.clickToCenter = clickItemToCenter;
				m_all.Add(item);
				++i;
			}

			m_end = -((RectTransform)content.GetChild(0).transform).anchoredPosition;
			SetContentAnchoredPosition(m_end);

			m_all.Sort();
			for (i = 0; i < m_totalPage + 1; i++)
			{
				CenterViewItem item = m_all[i] as CenterViewItem;
				((RectTransform)item.transform).SetSiblingIndex(i);
				Vector3 v = item.transform.localPosition;
				v.z = i;
				item.transform.localPosition = v;
			}
			m_all.Clear();

			if (horizontal)
			{
				_scaleXRenders();
			}
			else if (vertical)
			{
				_scaleYRenders();
			}

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
            //记录开始拖动时内容的位置和开始拖动时的时间戳.
            m_contentPos = content.anchoredPosition;
			m_dragTime = Time.realtimeSinceStartup;
            base.OnBeginDrag(eventData);
			if(onBeginDragAction!=null) onBeginDragAction(eventData.delta);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 1) return;
			Graphic g = GetComponent<Graphic>();
			if (g && g.raycastTarget)
				base.OnDrag(eventData);
			if(onDragAction!=null) onDragAction(eventData.delta);
        }

        override public void OnEndDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 1) return;
			Graphic g = GetComponent<Graphic>();
			if (g==null || g.raycastTarget==false) return;

            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            m_drag = false;
            base.OnEndDrag(eventData);

            //遍历所有的，判断离哪个最近.
            if (horizontal)
            {
				if (Time.realtimeSinceStartup - m_dragTime < 0.4f)
                {
                    //时间很短.
                    m_end.x += (eventData.delta.x + content.anchoredPosition.x - m_contentPos.x)*2f;
                }
                else if (Mathf.Abs(eventData.delta.x) > 5f)
                {
                    m_end.x += eventData.delta.x + content.anchoredPosition.x - m_contentPos.x;
                }
                else
                {
                    m_end = content.anchoredPosition;
                }

                float minDistance = float.MaxValue;
                foreach (RectTransform child in content)
                {
                    float pos = child.anchoredPosition.x + m_end.x;
                    float dis = Mathf.Abs(pos);
                    if (dis < minDistance)
                    {
                        minDistance = dis;
                        m_centerItem = child;
                    }
                }
                if (m_centerItem)
                {
                    m_end.x = -m_centerItem.anchoredPosition.x;
                    if (Mathf.Abs(m_end.x- content.anchoredPosition.x) > 1f)
                    {
                        _enableUpdate = true;
                        m_currentPage = m_centerItem.GetComponent<CenterViewItem>().index;
                        onPageChange.Invoke();
                        if (pageIndicator)
                            pageIndicator.ShowPage(m_currentPage);
                    }
                }
                
            }
            else if (vertical)
            {
				if (Time.realtimeSinceStartup - m_dragTime < 0.4f)
                {
                    //时间很短.
                    m_end.y += (eventData.delta.y + content.anchoredPosition.y - m_contentPos.y)*2f;
                }
                else if (Mathf.Abs(eventData.delta.y) > 5f)
                {
                    m_end.y += eventData.delta.y + content.anchoredPosition.y - m_contentPos.y;
                }
                else
                {
                    m_end = content.anchoredPosition;
                }

                float minDistance = float.MaxValue;
                foreach (RectTransform child in content)
                {
                    float pos = child.anchoredPosition.y + m_end.y;
                    float dis = Mathf.Abs(pos);
                    if (dis < minDistance)
                    {
                        minDistance = dis;
                        m_centerItem = child;
                    }
                }
                if (m_centerItem)
                {
                    m_end.y = -m_centerItem.anchoredPosition.y;
                    if (Mathf.Abs(m_end.y - content.anchoredPosition.y) > 1f)
                    {
                        _enableUpdate = true;
                        m_currentPage = m_centerItem.GetComponent<CenterViewItem>().index;
                        onPageChange.Invoke();
                        if (pageIndicator)
                            pageIndicator.ShowPage(m_currentPage);
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
        /// 用来排序.
        /// </summary>
        class Comp : IComparable
        {
            public RectTransform rect;
            public int CompareTo(object other)
            {
                float result = rect.localScale.x*100f - (other as Comp).rect.localScale.x*100f;
                if (result > 0) return 1;
                else if (result < 0) return -1;
                return 0;
            }
        }

        private void _scaleXRenders()
        {
            //遍历所有的，判断离哪个最近.
            foreach (RectTransform child in content)
            {
                var posX = child.anchoredPosition.x + content.anchoredPosition.x;
                float sc =Mathf.Abs( Mathf.Sin((child.sizeDelta.x * 4f - Mathf.Abs(posX)) / child.sizeDelta.x / 4f) * maxScale);
                if (posX > viewRect.sizeDelta.x  || posX < -viewRect.sizeDelta.x )
                {
                    sc = minScale;
                }
                child.localScale = new Vector2(sc, sc);
                Comp comp = new Comp();
                comp.rect = child;
                m_all.Add(comp);
            }
            m_all.Sort();
            for (int i = 0; i < m_totalPage + 1; i++)
            {
                Comp comp = m_all[i] as Comp;
                comp.rect.SetSiblingIndex(i);

				Vector3 v = comp.rect.transform.localPosition;
				v.z = i;
				comp.rect.transform.localPosition = v;
            }
            m_all.Clear();
        }


        private void _scaleYRenders()
        {
            //遍历所有的，判断离哪个最近.
            foreach (RectTransform child in content)
            {
                var posY = child.anchoredPosition.y + content.anchoredPosition.y;
                float sc = Mathf.Abs(Mathf.Sin((child.sizeDelta.y * 4f - Mathf.Abs(posY)) / child.sizeDelta.y / 4f) * maxScale);
                if (posY > viewRect.sizeDelta.y || posY < -viewRect.sizeDelta.y)
                {
                    sc = minScale;
                }
                child.localScale = new Vector2(sc, sc);
                Comp comp = new Comp();
                comp.rect = child;
                m_all.Add(comp);
            }
            m_all.Sort();
            for (int i = 0; i < m_totalPage + 1; i++)
            {
                Comp comp = m_all[i] as Comp;
                comp.rect.SetSiblingIndex(i);

				Vector3 v = comp.rect.transform.localPosition;
				v.z = i;
				comp.rect.transform.localPosition = v;
            }
            m_all.Clear();
        }


        override protected void LateUpdate()
        {
            if (m_drag)
            {
                base.LateUpdate();
                if (horizontal)
                {
                    _scaleXRenders();
                }
                else if (vertical)
                {
                    _scaleYRenders();
                }
            }
            else if (_enableUpdate)
            {
                if (horizontal)
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
                    _scaleXRenders();
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
                    _scaleYRenders();
                }
            }
        }

        /// <summary>
        /// 是否还有下一页.
        /// </summary>
        /// <returns></returns>
        public bool HasNextPage()
        {
            return m_currentPage < m_totalPage;
        }
        /// <summary>
        /// 是否有上一页.
        /// </summary>
        /// <returns></returns>
        public bool HasPrevPage()
        {
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
        /// <param name="anim">是否有缓动画.默认为true</param>
        public void GotoPage(int pageIndex,bool anim=true)
        {
            if (m_currentPage != pageIndex)
            {
                m_currentPage = pageIndex;
                RectTransform item = FindItemByIndex(m_currentPage);
                if (item)
                {
                    m_centerItem = item;
                    Vector2 endPos = -item.anchoredPosition;
                    m_end = endPos;
                    _enableUpdate = true;
                    onPageChange.Invoke();
                    if (pageIndicator)
                        pageIndicator.ShowPage(m_currentPage);

                    if (!anim)
                    {
                        _enableUpdate = false;
                        SetContentAnchoredPosition(m_end);
                        if (horizontal)
                        {
                            _scaleXRenders();
                        }
                        else if (vertical)
                        {
                            _scaleYRenders();
                        }
                    }
                }
            }
        }

        public RectTransform FindItemByIndex(int index)
        {
            foreach (RectTransform child in content)
            {
                if (child.GetComponent<CenterViewItem>().index == index)
                {
                    return child;
                }
            }
            return null;
        }
    }
}


