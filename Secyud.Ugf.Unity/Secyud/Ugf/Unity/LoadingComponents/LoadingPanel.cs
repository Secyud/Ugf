using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.LoadingComponents
{
    [RequireComponent(typeof(Canvas))]
    public class LoadingPanel : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private float _speed = 100;

        private ILoadableObject _progressRate;

        protected virtual void Awake()
        {
            GetComponent<Canvas>().worldCamera = U.Camera;
            _slider.minValue = 0;
            _slider.maxValue = 100;
            _slider.value = 0;
        }

        private void Update()
        {
            if (_slider.value >= 100)
            {
                Destroy(gameObject);
            }
            else if (_slider.value < _progressRate.Rate)
            {
                _slider.value = Math.Min(_speed * Time.deltaTime, _progressRate.Rate);
                if (_text)
                {
                    _text.text = $"{Math.Min(100, (int)_slider.value)}%";
                }
            }
        }
    }
}