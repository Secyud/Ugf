using System;
using Secyud.Ugf.Unity.LoadingComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.UiForms
{
    public class LoadingFormBase<TForm>:UiFormBase<TForm>
        where TForm : UiFormBase
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private float _speed = 100;

        protected IProgressRate ProgressRate { get; set; }

        protected virtual void Awake()
        {
            _slider.minValue = 0;
            _slider.maxValue = 100;
            _slider.value = 0;
        }

        protected virtual void Update()
        {
            if (_slider.value >= 90 && ProgressRate.LoadFinished)
            {
                DestroyFrom();
            }
            else if (_slider.value < ProgressRate.Rate)
            {
                _slider.value = Math.Min(_speed * Time.deltaTime, ProgressRate.Rate);
                if (_text)
                {
                    _text.text = $"{Math.Min(100, (int)_slider.value)}%";
                }
            }
        }
    }
}