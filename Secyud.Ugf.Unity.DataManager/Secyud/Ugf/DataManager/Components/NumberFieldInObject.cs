using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.DataManager.Components
{
    public class NumberFieldInObject : FieldInObject
    {
        [SerializeField] protected TMP_InputField NumberInput;
        [SerializeField] protected Image InvalidIcon;

        protected override void BindValue(object value)
        {
            NumberInput.SetTextWithoutNotify(value.ToString());
        }

        public void SetNumber(string str)
        {
            bool failed = false;
            try
            {
                SetValue(Convert.ChangeType(str, Field.Info.FieldType));
            }
            catch (Exception)
            {
                failed = true;
            }

            InvalidIcon.enabled = failed;
        }
    }
}