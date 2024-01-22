using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.Unity.Ui.Notifications
{
    public class NotificationContent : MonoBehaviour
    {
        private LinkedList<Notification> _notifications;
        [SerializeField] private Notification _templatePrefab;

        private void Awake()
        {
            _notifications = new LinkedList<Notification>();
        }

        internal void RemoveNotification(Notification notification)
        {
            _notifications.Remove(notification);
        }

        internal void AddNotification(Notification notification)
        {
            int i = 0;
            foreach (Notification n in _notifications)
            {
                n.MoveTo(++i);
            }

            _notifications.AddFirst(notification);
        }

        public Notification CreateNotification()
        {
            Notification ret = _templatePrefab.Instantiate(transform);
            ret.Initialize(this);
            return ret;
        }
    }
}