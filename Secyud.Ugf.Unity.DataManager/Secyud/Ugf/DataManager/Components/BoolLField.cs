using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.DataManager.Components
{
    public class BoolLField : ListItem
    {
        [SerializeField] private Toggle _toggle;

        public override void Bind(ListField parent, int index)
        {
            base.Bind(parent, index);
            _toggle.SetIsOnWithoutNotify((bool)parent.List[index]);
        }

        public void SetValue(bool b)
        {
            Parent.List[Index] = b;
        }
    }
}