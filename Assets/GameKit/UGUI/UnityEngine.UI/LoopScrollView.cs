using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Loop Scroll View", 1352)]
	public class LoopScrollView : ScrollRect, IPointerUpHandler{

		private Vector2 m_Gap;
		private Vector2 m_DragDelta;

		public override void Rebuild(CanvasUpdate executing)
		{
			if (executing != CanvasUpdate.PostLayout)
				return;
			base.Rebuild(executing);

			if(content.childCount>1){
				m_Gap = (content.GetChild(1) as RectTransform).anchoredPosition - (content.GetChild(0) as RectTransform).anchoredPosition;
			}

			Vector2 v = -((RectTransform)transform).sizeDelta/2;
			v.y = -v.y;
			SetContentAnchoredPosition(v);
			movementType = MovementType.Unrestricted;

			if(Application.isPlaying){

				LayoutGroup group = transform.GetComponentInChildren<LayoutGroup>();
				if(group){

//					Vector2 itemPos = Vector2.one;
//					Vector2 itemSize = Vector2.one;
//					Vector2 itemAnchorMin = Vector2.one;
//					Vector2 itemAnchorMax = Vector2.one;
//					Vector2 itemPivot = Vector2.one;
//					Vector2 itemOffsetMin = Vector2.one;
//					Vector2 itemOffsetMax = Vector2.one;
//					List<Vector2> pos = new List<Vector2>();
//					if (content.childCount > 0) {
//						RectTransform child = content.GetChild (0).transform as RectTransform;
//						itemSize = child.sizeDelta;
//						itemPos = child.anchoredPosition;
//						itemAnchorMin = child.anchorMin;
//						itemAnchorMax = child.anchorMax;
//						itemPivot = child.pivot;
//						itemOffsetMin = child.offsetMin;
//						itemOffsetMax = child.offsetMax;
//						foreach (RectTransform ch in content)
//						{
//							pos.Add(ch.anchoredPosition);
//						}
//					}

					group.enabled = false;

//					int i = 0;
//					foreach (RectTransform child in content)
//					{
//						itemPivot = child.pivot;
//						child.offsetMin = itemOffsetMin;
//						child.offsetMax = itemOffsetMax;
//						child.anchorMin = itemAnchorMin;
//						child.anchorMax = itemAnchorMax;
//						child.sizeDelta = itemSize;
//						child.anchoredPosition = pos[i];
//						++i;
//
//						LayoutGroup childGroup = child.GetComponent<LayoutGroup>();
//						if(childGroup && childGroup.enabled){
//							childGroup.enabled = false;
//							childGroup.enabled = true;
//						}
//					}
				}
			}

			onValueChanged.RemoveListener(CheckLoop);
			onValueChanged.AddListener(CheckLoop);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			OnEndDrag(eventData);
			m_DragDelta = eventData.delta;
		}

		public override void OnDrag(PointerEventData eventData)
		{
			base.OnDrag(eventData);
			m_DragDelta = eventData.delta;
		}

		/// <summary>
		/// 手动更新Loop
		/// </summary>
		/// <param name="delta">Delta.</param>
		public void UpdateLoop(Vector2 delta){
			m_DragDelta = delta;
			CheckLoop(m_DragDelta);
		}

		void CheckLoop(Vector2 delta){

			if(horizontal){
				if(m_DragDelta.x>=0){
					RectTransform child = content.GetChild(0) as RectTransform;
					if(child.anchoredPosition.x + content.anchoredPosition.x>-m_Gap.x){
						RectTransform lastChild = content.GetChild(content.childCount-1) as RectTransform;
						Vector2 v = lastChild.anchoredPosition;
						v.x = child.anchoredPosition.x - m_Gap.x;
						lastChild.anchoredPosition = v;
						lastChild.SetAsFirstSibling();
					}
				}

				if(m_DragDelta.x<=0){
					RectTransform child = content.GetChild(content.childCount-1) as RectTransform;
					if(child.anchoredPosition.x + content.anchoredPosition.x<m_Gap.x*2){
						RectTransform firstChild = content.GetChild(0) as RectTransform;
						Vector2 v = firstChild.anchoredPosition;
						v.x = child.anchoredPosition.x + m_Gap.x;
						firstChild.anchoredPosition = v;
						firstChild.SetAsLastSibling();
					}
				}
			}
			else if(vertical)
			{
				if(m_DragDelta.y<=0){
					RectTransform child = content.GetChild(0) as RectTransform;
					if(child.anchoredPosition.y + content.anchoredPosition.y<-m_Gap.y ){
						RectTransform lastChild = content.GetChild(content.childCount-1) as RectTransform;
						Vector2 v = lastChild.anchoredPosition;
						v.y = child.anchoredPosition.y - m_Gap.y;
						lastChild.anchoredPosition = v;
						lastChild.SetAsFirstSibling();
					}
				}

				if(m_DragDelta.y>=0){
					RectTransform child = content.GetChild(content.childCount-1) as RectTransform;
					if(child.anchoredPosition.y + content.anchoredPosition.y>m_Gap.y*2){
						RectTransform firstChild = content.GetChild(0) as RectTransform;
						Vector2 v = firstChild.anchoredPosition;
						v.y = child.anchoredPosition.y + m_Gap.y;
						firstChild.anchoredPosition = v;
						firstChild.SetAsLastSibling();
					}
				}
			}
		}
	}
}