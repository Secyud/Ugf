#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public class SImage : Image
    {
        public void Set(Sprite img)
        {
            sprite = img ? img : Og.EmptyImage;
        }
    }
}