#region

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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