using DG.Tweening;
using UnityEngine;

namespace Secyud.Ugf.NotificationComponents
{
    /// <summary>
    /// 悬浮提示框，在一定时间后消失，并让后面的上移。
    /// </summary>
    [RequireComponent(
        typeof(RectTransform),
        typeof(CanvasGroup))]
    public class Notification : MonoBehaviour
    {
        [SerializeField] private float ExistTime;
        [SerializeField] private float Interval = 4;
        private NotificationContent _content;
        private RectTransform _transform;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _transform = GetComponent<RectTransform>();
        }

        public void OnInitialize(NotificationContent content)
        {
            _content = content;
            _transform.SetParent(content.transform);
            _transform.localPosition = Vector3.zero;
            gameObject.SetActive(true);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(_canvasGroup.DOFade(1, 0.2f));
            sequence.AppendInterval(ExistTime);
            sequence.Append(_canvasGroup.DOFade(0, 0.8f));
            sequence.OnComplete(Die);
            sequence.Play();

            _content.AddNotification(this);
        }

        public void Die()
        {
            Destroy(this);
        }

        private void OnDestroy()
        {
            _content.RemoveNotification(this);
        }

        public void MoveTo(int index)
        {
            _transform.DOLocalMoveY( index * (_transform.rect.height + Interval), 0.3f);
        }
    }
}