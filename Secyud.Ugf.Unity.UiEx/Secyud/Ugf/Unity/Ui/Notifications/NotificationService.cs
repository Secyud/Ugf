using System;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Unity.AssetLoading;
using UnityEngine;

namespace Secyud.Ugf.Unity.Ui.Notifications
{
    public class NotificationService : IRegistry
    {
        private readonly MessageService _messageService;
        public PrefabContainer<NotificationPanel> NotificationPrefab { get; set; }

        private NotificationPanel _instance;

        /// <summary>
        /// Popup a notification.
        /// Do something to fill the transform.
        /// </summary>
        /// <param name="action">
        /// Action to set the content of notification
        /// </param>
        /// <exception cref="UgfNotRegisteredException">
        /// Throw if the prefab of notification panel is
        /// not registered.
        /// </exception>
        public void PopupNotification(Action<RectTransform> action)
        {
            if (_instance)
            {
                action?.Invoke(_instance.CreateNotification());
            }
            else
            {
                if (NotificationPrefab is null)
                {
                    throw new UgfNotRegisteredException(
                        nameof(NotificationService),
                        nameof(NotificationPrefab));
                }

                MessageService messageService = U.Get<MessageService>();
                Canvas canvas = messageService.MessageCanvas;

                NotificationPrefab.GetValueAsync(p =>
                {
                    _instance = p.Instantiate(canvas.transform);
                    action?.Invoke(_instance.CreateNotification());
                });
            }
        }
    }
}