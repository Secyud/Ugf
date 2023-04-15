#region

using System;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    [RequireComponent(typeof(Canvas))]
    public class LoadingPanel : MonoBehaviour
    {
        [SerializeField] private SSlider Slider;
        [SerializeField] private SText Text;
        [SerializeField] private float Speed;

        private LoadingService _service;

        private void Awake()
        {
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