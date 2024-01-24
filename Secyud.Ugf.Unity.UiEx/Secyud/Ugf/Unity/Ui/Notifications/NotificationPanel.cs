using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Secyud.Ugf.Unity.Ui.Notifications
{
    public class NotificationPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _templatePrefab;
        [SerializeField] private float _existTime;
        [SerializeField] private float _interval;
        [SerializeField] private float _moveTime = 0.3f;
        [SerializeField] private int _maxNotificationCount = 9;
        private List<RectTransform> _showNotifications;
        private List<RectTransform> _idleNotifications;

        private void Awake()
        {
            _showNotifications = new List<RectTransform>();
            _idleNotifications = new List<RectTransform>();
        }

        public RectTransform CreateNotification()
        {
            RectTransform notification;
            if (_idleNotifications.Count > 0)
            {
                notification = _idleNotifications.Last();
                _idleNotifications.RemoveAt(_idleNotifications.Count - 1);
            }
            else
            {
                if (_showNotifications.Count < _maxNotificationCount)
                {
                    notification = _templatePrefab.Instantiate(transform);
                }
                else
                {
                    notification = _showNotifications[0];
                    _showNotifications.RemoveAt(0);
                }
            }

            float height = 0;
            for (int i = _showNotifications.Count - 1; i >= 0; i--)
            {
                RectTransform exist = _showNotifications[i];
                height += exist.rect.height + _interval;
                _showNotifications[i].DOLocalMoveY(height, _moveTime);
            }

            _showNotifications.Add(notification);
            notification.localPosition = Vector3.zero;
            notification.gameObject.SetActive(true);

            CanvasGroup canvasGroup = notification
                .GetOrAddComponent<CanvasGroup>();
            Sequence sequence = DOTween.Sequence();
            sequence.Append(canvasGroup.DOFade(1, 0.2f));
            sequence.AppendInterval(_existTime);
            sequence.Append(canvasGroup.DOFade(0, 0.8f));
            sequence.OnComplete(() => RecycleNotification(notification));
            sequence.Play();

            return notification;
        }

        private void RecycleNotification(RectTransform notification)
        {
            _showNotifications.Remove(notification);
            _idleNotifications.Add(notification);
            notification.gameObject.SetActive(false);
        }
    }
}