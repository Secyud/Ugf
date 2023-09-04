using Secyud.Ugf.BasicComponents;
using UnityEngine;

namespace Secyud.Ugf.TableComponents.FilterComponents
{
    public class FilterPopup:SPopup
    {
        public static FilterPopup PopupExist;
        
        public void Close()
        {
            if(PopupExist)
                PopupExist.Destroy();
        }

        public RectTransform Open()
        {
            Close();
            PopupExist = this.InstantiateOnCanvas();
            PopupExist.InitializeOnMouse();
            return PopupExist.PrepareLayout();
        }
    }
}