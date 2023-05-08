#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.BasicComponents
{
    public class SImage : Image
    {
        public virtual Sprite Sprite
        {
            get => sprite;
            set
            {
                sprite = value;
                enabled = sprite;
            }
        }
    }
}