#region

using Secyud.Ugf.Layout;
using UnityEngine;

#endregion

namespace Secyud.Ugf.BasicComponents
{
    [RequireComponent(typeof(RectTransform))]
    public class SFloating : VerticalLayoutTrigger
    {
        [SerializeField] private RectTransform MsgContent;

        public void OnInitialize(Vector2 position)
        {
            RectTransform.anchoredPosition = position;
        }
        
        public SFloating Create(Vector2 position)
        {
            var floating = Instantiate(this, Og.Canvas.transform);
            floating.OnInitialize(position);
            return floating;
        }

        public SFloating CreateOnMouse()
        {
            var floating = Create(UgfUnityExtensions.GetMousePosition());
            return floating;
        }

        public RectTransform MContent => MsgContent;

        public void RefreshLayout()
        {
            enabled = true;
        }
        
        public void Die()
        {
            Destroy(gameObject);
        }
    }
}