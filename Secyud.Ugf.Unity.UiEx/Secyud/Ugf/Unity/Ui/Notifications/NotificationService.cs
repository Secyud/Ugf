using System;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Unity.AssetLoading;
using UnityEngine;

namespace Secyud.Ugf.Unity.Ui.Notifications
{
    public class NotificationService : IRegistry
    {
        public MonoContainer<NotificationPanel> NotificationPanel { get; }

        public NotificationService(MessageService messageService)
        {
            NotificationPanel = new MonoContainer<NotificationPanel>(
                messageService.MessageCanvas.transform);
        }

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
            NotificationPanel.GetValueAsync(p =>
                action?.Invoke(p.CreateNotification()));
        }
    }
}