using Secyud.Ugf.BasicComponents;
using UnityEngine;

namespace Secyud.Ugf.TableComponents.FilterComponents
{
    public class FilterPopup:SPopup
    {
        private static FilterPopup _popupExist;
        
        public void Close()
        {
            if(_popupExist)
                _popupExist.Destroy();
        }

        public RectTransform Open()
        {
            Close();
            _popupExist = this.InstantiateOnCanvas();
            _popupExist.InitializeOnMouse();
            return _popupExist.PrepareLayout();
        }
    }
}