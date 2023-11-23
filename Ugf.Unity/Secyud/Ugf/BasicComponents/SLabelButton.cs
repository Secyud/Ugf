#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.BasicComponents
{
    public class SLabelButton : SButton
    {
        [SerializeField] private SText Label;

        public string Text
        {
            get => Label.text;
            set => Label.text = value;
        }
    }
}