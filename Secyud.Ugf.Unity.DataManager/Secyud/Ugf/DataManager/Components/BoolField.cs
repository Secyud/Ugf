using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.DataManager.Components
{
    public class BoolField : DataField
    {
        [SerializeField] private Toggle _toggle;

        public override void Bind(object parent, SAttribute sAttribute)
        {
            base.Bind(parent, sAttribute);
            _toggle.SetIsOnWithoutNotify((bool)sAttribute.GetValue(parent));
        }

        public void SetValue(bool b)
        {
            SAttribute.SetValue(Parent, b);
        }
    }
}