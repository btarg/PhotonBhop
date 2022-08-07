using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace UIComponents {
	/// <summary>
	/// 
	/// </summary>
	public class GridBuilder : GridCell {
		[Tooltip("Automatically re-size grid every frame to reflect changes in children")]
		public bool _autoResize = true;

		[Tooltip("Resize grid width to match content")]
		public bool _resizeWidth = true;

		[Tooltip("Resize grid height to match content")]
		public bool _resizeHeight = true;

		/*
		[Tooltip("Default alignment for full rows (individual cells are always left aligned)")]
		public Alignment _defaultRowAlignment = Alignment.Left;
		*/
		public float layoutHeight => -_nextY;
		public float layoutWidth => _rowWidth;
		public float layoutOffsetX => _nextX;

		public float maxLayoutHeight { get; set; }
		public float maxLayoutWidth { get; set; }

		public float minLayoutWidth { get; set; }
		public float minLayoutHeight { get; set; }

		private float _rowWidth = 0;
		private float _rowHeight = 0;
		private float _nextX = 0;
		private float _nextY = 0;
		private int _childCounter;
		private List<GridCell> _currentChildren = new List<GridCell>();

		void Awake() {
			OnRecycled();
		}

		public override void OnRecycled() {
			base.OnRecycled();
			maxLayoutHeight = float.MaxValue;
			maxLayoutWidth = float.MaxValue;

			float startW = 0;
			float startH = 0;

			if (transform.parent) {
				RectTransform rt = rectTransform;

				if (!_resizeWidth)
					startW = (rt.anchorMax.x - rt.anchorMin.x) * ((RectTransform) transform.parent).rect.width;
				if (!_resizeHeight)
					startH = (rt.anchorMax.y - rt.anchorMin.y) * ((RectTransform) transform.parent).rect.height;
			}

			minLayoutWidth = startW;
			minLayoutHeight = startH;

			// NOTE: Do not call UpdateRect() here - it breaks when called during Awake(). Wait for first Update() or Begin/End sequence before changing anything
		}

		public void BeginUpdate() {
			if (_currentChildren.Count != transform.childCount) {
				_currentChildren.Clear();
				for (int c = 0; c < transform.childCount;) {
					GameObject go = transform.GetChild(c).gameObject;
					var po = go.GetComponent<PooledObject>();
					if (po != null)
						ObjectPool.Recycle(po);
					else {
						// Since we can't recycle non pooled objects, we need to skip them. This should not really happen, but could if someone adds children manually, and we don't want to get stuck in an endless loop
						c++;
//						po.transform.SetParent(null);
//				    GameObject.Destroy(go);
					}
				}
			}

			_childCounter = 0;
			_nextX = 0;
			_nextY = 0;
			_rowWidth = 0;
			_rowHeight = 0;
		}

		public void AddRowSpacing(float spc) {
			if (spc > 0)
				AddRow(GetSpacerPrefab(), (GridCell spacer) => {
					spacer.width = 0;
					spacer.height = spc;
				});
		}

		public void AddCellSpacing(float spc) {
			if (spc > 0)
				AddCell(GetSpacerPrefab(), (GridCell spacer) => {
					spacer.width = spc;
					spacer.height = 0;
				});
		}

		public enum Alignment {
			Default,
			Left,
			Center,
			Right
		}

		public T AddRow<T>(T prefab, Action<T> setup = null, Alignment align = Alignment.Default) where T : GridCell {
			/*
			if (align == Alignment.Default)
				align = _defaultRowAlignment;
			float pw = (prefab.rectTransform.anchorMax.x - prefab.rectTransform.anchorMin.x) * width;
			if (pw <= 0)
				pw = prefab.width;
			switch (align)
			{
				case Alignment.Center:
					AddCellSpacing( (width - pw)/2.0f);
					break;
				case Alignment.Right:
					AddCellSpacing( width - pw);
					break;
			}
			*/
			return AddCell(prefab, setup, true);
		}

		public T AddCell<T>(T prefab, Action<T> setup = null, bool endRow = false) where T : GridCell {
			T panel = Add(prefab, _nextX, _nextY);
			if (setup != null)
				setup(panel);

			panel.endRow = endRow;

			LayoutCell(panel);

			return panel;
		}

		public void EndRow() {
			if (_childCounter > 0 && !_currentChildren[_childCounter - 1].endRow) // If EndRow is set to true, we're already on a new row and don't need to do anything
			{
				_currentChildren[_childCounter - 1].endRow = true;
				MoveNextRow();
			}
		}

		public void EndUpdate() {
			EndRow(); // Make sure the last row is terminated so we get the correct parent height.
			for (int i = _childCounter; i < _currentChildren.Count;) {
				ObjectPool.Recycle(_currentChildren[i]);
				_currentChildren.RemoveAt(i);
			}

			UpdateRect(_rowWidth, -_nextY); // Make sure content pane is resized if needed!
		}

		private void UpdateRect(float w, float h) {
			if (_resizeWidth) width = Mathf.Max(minLayoutWidth, Mathf.Min(maxLayoutWidth, w));
			if (_resizeHeight) height = Mathf.Max(minLayoutHeight, Mathf.Min(maxLayoutHeight, h));
		}

		private void LayoutCell(GridCell cell) {
			cell.rectTransform.anchoredPosition = new Vector2(_nextX, _nextY);

			_nextX += cell.width;
			if (_nextX > _rowWidth)
				_rowWidth = _nextX;

			if (cell.height > _rowHeight)
				_rowHeight = cell.height;

			if (cell.endRow)
				MoveNextRow();
		}

		void MoveNextRow() {
			_nextY -= _rowHeight;
			_nextX = 0;
			_rowHeight = 0;
		}

		private static GridCell g_spacerPrefab;

		private GridCell GetSpacerPrefab() {
			if (g_spacerPrefab == null) {
				GameObject go = new GameObject("BNGridSpacer", typeof(RectTransform));
				g_spacerPrefab = go.AddComponent<GridCell>();
				RectTransform rt = (RectTransform) go.transform;
				rt.pivot = new Vector2(0, 1);
				rt.anchorMin = rt.pivot;
				rt.anchorMax = rt.pivot;
			}

			return g_spacerPrefab;
		}

		private T Add<T>(T prefab, float x, float y) where T : GridCell {
			GridCell panel;
			if (_childCounter < _currentChildren.Count && _currentChildren[_childCounter].pool.prefab == prefab) {
				panel = _currentChildren[_childCounter];
			}
			else {
				panel = ObjectPool.Instantiate(prefab);
//	      panel.pool.prefab = prefab;
				RectTransform rt = (RectTransform) panel.transform;
				rt.SetParent(transform, false);
				rt.localScale = Vector3.one;

				if (_childCounter < _currentChildren.Count) {
					ObjectPool.Recycle(_currentChildren[_childCounter]);
					_currentChildren[_childCounter] = panel;
				}
				else
					_currentChildren.Add(panel);
			}

			_childCounter++;

			panel.transform.localPosition = new Vector2(x, y);
//	    var t = panel.rectTransform.anchoredPosition;
			return (T) panel;
		}

		void Update() {
			if (_autoResize) {
				if (_resizeWidth || _resizeHeight) {
					_nextX = 0;
					_nextY = 0;
					_rowWidth = 0;
					_rowHeight = 0;
					foreach (GridCell cell in _currentChildren) {
						LayoutCell(cell);
					}

					if (_currentChildren.Count > 0)
						UpdateRect(_rowWidth, -_nextY); // Make sure content pane is resized if needed!
				}
			}
		}
	}
}