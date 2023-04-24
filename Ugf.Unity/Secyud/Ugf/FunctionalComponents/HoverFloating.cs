#region

using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Secyud.Ugf.FunctionalComponents
{
    // 常驻悬浮框用于聚焦时弹出
    // 用于悬浮菜单，当未聚焦时可以收缩在屏幕边缘
    [RequireComponent(typeof(RectTransform))]
    public class HoverFloating : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float Bias;
        [SerializeField] private float Speed;
        [SerializeField] private bool Reverse;
        [SerializeField] private bool Horizontal;
        [SerializeField] private Transform FixLabel;
        private float _biasRecord;
        private bool _fixed;
        private bool _isIn;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (_fixed) return;

            if (_isIn && _biasRecord < Bias)
                Move(Speed * Time.deltaTime);
            else if (!_isIn && _biasRecord > 0)
                Move(-Speed * Time.deltaTime);
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            _isIn = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isIn = false;
        }

        private void Move(float value)
        {
            value = Mathf.Clamp(value, -_biasRecord, Bias - _biasRecord);

            var realValue = value * (Reverse ? -1 : 1);
            _rectTransform.anchoredPosition += Horizontal
                ? new Vector2(realValue, 0f)
                : new Vector2(0f, realValue);
            _biasRecord += value;
        }

        public void SetFixed()
        {
            _fixed = !_fixed;

            if (FixLabel)
                FixLabel.rotation = _fixed
                    ? Quaternion.Euler(0, 0, 90)
                    : Quaternion.Euler(0, 0, 0);

            if (_fixed)
                Move(Bias - _biasRecord);
        }
    }
}