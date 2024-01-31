using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.DataManager.Components
{
    public class NumberLField : ListItem
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Image _wrong;

        public override void Bind(ListField parent, int index)
        {
            base.Bind(parent, index);
            _inputField.SetTextWithoutNotify(
                parent.List[index].ToString());
        }

        public void SetValue(string str)
        {
            bool failed = false;
            try
            {
                Parent.List[Index] = Convert.ChangeType(
                    str, Parent.SAttribute.Info.FieldType);
            }
            catch (Exception)
            {
                failed = true;
            }

            _wrong.enabled = failed;
        }
    }
}