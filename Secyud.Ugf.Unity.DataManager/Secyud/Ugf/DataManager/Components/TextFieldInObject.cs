using TMPro;
using UnityEngine;

namespace Secyud.Ugf.DataManager.Components
{
    public class TextFieldInObject : FieldInObject
    {
        [SerializeField] protected TMP_InputField TextInput;

        protected override void BindValue(object value)
        {
            TextInput.SetTextWithoutNotify(value as string);
        }

        public void SetString(string str)
        {
            SetValue(str);
        }
    }
}