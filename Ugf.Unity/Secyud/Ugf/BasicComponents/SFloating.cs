#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.BasicComponents
{
    [RequireComponent(typeof(RectTransform))]
    public class SFloating : MonoBehaviour
    {
        public RectTransform Content;
        public RectTransform RectTransform { get; private set; }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public void OnInitialize(Vector2 position)
        {
            RectTransform.anchoredPosition = position;
        }

        public void CheckBoundary()
        {
            RectTransform.CheckBoundary();
        }

        public SFloating Create(Vector2 position)
        {
            var floating = Instantiate(this, Og.Canvas.transform);
            floating.OnInitialize(position);
            return floating;
        }

        public SFloating CreateOnMouse()
        {
            var floating = Create(UnityExtension.GetMousePosition() + new Vector2(-16, 8));
            return floating;
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}