using TMPro;
using UnityEngine;

namespace Secyud.Ugf.DataManager.Components
{
    public class TextField : DataField
    {
        [SerializeField] private TMP_InputField _inputField;

        public override void Bind(object parent, SAttribute field)
        {
            base.Bind(parent, field);
            _inputField.SetTextWithoutNotify(
                field.GetValue(parent)?.ToString());
        }

        public void SetValue(string str)
        {
            SAttribute.SetValue(Parent, str);
        }
    }
}