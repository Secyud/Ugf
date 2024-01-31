using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.DataManager.Components
{
    public class NumberField : DataField
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Image _wrong;

        public override void Bind(object parent, SAttribute sAttribute)
        {
            base.Bind(parent, sAttribute);
            _inputField.SetTextWithoutNotify(
                sAttribute.GetValue(parent).ToString());
        }

        public void SetValue(string str)
        {
            bool failed = false;
            try
            {
                SAttribute.SetValue(Parent, 
                    Convert.ChangeType(str, SAttribute.Info.FieldType));
            }
            catch (Exception)
            {
                failed = true;
            }

            _wrong.enabled = failed;
        }
    }
}