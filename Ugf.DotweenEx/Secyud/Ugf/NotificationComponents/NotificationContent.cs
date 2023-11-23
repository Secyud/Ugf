using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.NotificationComponents
{
    public class NotificationContent:MonoBehaviour
    {
        private LinkedList<Notification> _notifications;
        [SerializeField] private Notification TemplatePrefab;
        
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

        public Notification CreateNew()
        {
            return TemplatePrefab.Instantiate(transform);
        }
    }
}