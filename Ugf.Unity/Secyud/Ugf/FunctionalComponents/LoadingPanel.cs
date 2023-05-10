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

		public static LoadingPanel Instance { get; private set; }

		private LoadingService _service;


		private void Awake()
		{
			if (Instance)
				Destroy(Instance.gameObject);
			Instance = this;
			Slider.minValue = 0;
			Slider.maxValue = 100;
			Slider.value = 0;
			GetComponent<Canvas>().worldCamera = Og.MainCamera;
			_service = Og.LoadingService;
		}

		private void Update()
		{
			if (Slider.value >= 100)
			{
				_service.Value = 0;
				Destroy(gameObject);
			}
			else if (Slider.value * _service.MaxValue <= _service.Value * 100)
			{
				Slider.value += Speed / 64;
				if (Text)
					Text.text = $"{Math.Min(100, (int)Slider.value)}%";
			}
		}
	}
}