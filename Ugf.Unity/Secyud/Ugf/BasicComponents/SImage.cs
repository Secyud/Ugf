#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.BasicComponents
{
    public class SImage : Image
    {
        public void Set(Sprite img)
        {
            sprite = img ? img : Og.EmptyImage;
        }
    }
}