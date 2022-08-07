using UnityEngine;
using Utility;

namespace UIComponents {
	public class GridCell : PooledObject {
		public bool endRow { get; set; }

		public RectTransform rectTransform {
			get { return (RectTransform) transform; }
		}

		public float width {
			set { ((RectTransform) transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value); }
			get { return ((RectTransform) transform).rect.width; }
		}

		public float height {
			set { ((RectTransform) transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value); }
			get { return ((RectTransform) transform).rect.height; }
		}

		public Vector2 offset {
			get { return new Vector2(rectTransform.anchoredPosition.x, -rectTransform.anchoredPosition.y); }
		}

		public void FillParent(RectTransform parent = null, float marginx = 0, float marginy = 0) {
			// if(parent==null)
			parent = (RectTransform) transform.parent;
			height = parent.rect.height + ((RectTransform) transform).anchoredPosition.y + marginy;
			width = parent.rect.width - ((RectTransform) transform).anchoredPosition.x - marginx;
		}
	}
}