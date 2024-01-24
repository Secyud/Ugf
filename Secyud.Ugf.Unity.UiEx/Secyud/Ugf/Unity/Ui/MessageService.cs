using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.Unity.Ui
{
    public class MessageService : IRegistry
    {
        private Canvas _messageCanvas;
        public Canvas MessageCanvas => _messageCanvas ??= CreateMessageCanvas();

        protected virtual Canvas CreateMessageCanvas()
        {
            GameObject gameObject = new("Message Canvas");
            gameObject.transform.SetParent(UgfGameManager.Instance.transform);
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.planeDistance = 935.3075f;
            canvas.worldCamera = U.Camera;
            return canvas;
        }
    }
}