#region

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#endregion

namespace Secyud.Ugf.FunctionalComponents
{
	public sealed class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private float Delay = 0.8f;
		[SerializeField] private UnityEvent OnHoverEvent = new();
		private float _hoverTime;
		private bool _hovered;
		private Vector2 _mouse;

		public UnityEvent OnHover
		{
			get => OnHoverEvent;
			set => OnHoverEvent = value;
		}

		private void Update()
		{
			if (!_hovered)
				return;
			
			if (Input.GetMouseButtonDown(0))
				ResetHover();
			else
			{
				Vector2 mouse = Input.mousePosition;

				if (Mathf.Approximately(mouse.x, _mouse.x) &&
					Mathf.Approximately(mouse.y, _mouse.y))
				{
					_hoverTime += Time.deltaTime;

					if (_hoverTime > Delay)
					{
						OnHoverEvent.Invoke();

						ResetHover();
					}
				}
				else
				{
					_hoverTime = 0;
				}
				_mouse = mouse;
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_hovered = true;
			_hoverTime = 0;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			ResetHover();
		}

		private void ResetHover()
		{
			_hovered = false;
			_hoverTime = 0;
		}

		public void Bind(UnityAction action)
		{
			OnHoverEvent.AddListener(action);
		}
	}
}