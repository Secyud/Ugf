using UnityEngine;

namespace Secyud.Ugf.Unity.Ui
{
    public class AutoCloseFloat:BasicFloat
    {
        public RectTransform CheckRange;
        
        protected override void CheckMouse()
        {
            if (CheckRange is null || 
                RectTransformUtility.RectangleContainsScreenPoint(CheckRange,Input.mousePosition))
                return;
            
            Destroy(gameObject);
        }
    }
}