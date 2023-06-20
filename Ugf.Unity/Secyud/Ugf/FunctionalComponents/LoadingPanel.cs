#region

using Secyud.Ugf.BasicComponents;
using Secyud.Ugf.Modularity;
using System;
using UnityEngine;

#endregion

namespace Secyud.Ugf.FunctionalComponents
{
	[RequireComponent(typeof(Canvas))]
	public class LoadingPanel : MonoBehaviour
	{
		[SerializeField] private SSlider Slider;
		[SerializeField] private SText Text;
		[SerializeField] private float Speed;


		private IUgfApplication _application;
		public static LoadingPanel Instance { get; private set; }

		public Action DestroyAction;

		private void OnDestroy()
		{
			DestroyAction?.Invoke();
		}

		protected virtual void Awake()
		{
			if (Instance)
				Destroy(Instance.gameObject);
			Instance = this;
			Slider.minValue = 0;
			Slider.maxValue = 100;
			Slider.value = 0;
			GetComponent<Canvas>().worldCamera = U.Camera;
			_application = U.Get<IUgfApplication>();
		}

		private void Update()
		{
			if (Slider.value >= 100)
			{
				_application.CurrentStep = 0;
				Destroy(gameObject);
			}
			else if (Slider.value * _application.TotalStep <= _application.CurrentStep * 100)
			{
				Slider.value += Speed / 64;
				if (Text)
					Text.text = $"{Math.Min(100, (int)Slider.value)}%";
			}
		}
	}
}