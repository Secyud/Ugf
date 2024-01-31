using TMPro;
using UnityEngine;

namespace Secyud.Ugf.DataManager.Components
{
    public class TextLField : ListItem
    {
        [SerializeField] private TMP_InputField _inputField;

        public override void Bind(ListField parent, int index)
        {
            _inputField.SetTextWithoutNotify(
                parent.List[index]?.ToString());
        }

        public void SetValue(string str)
        {
            Parent.List[Index] = str;
        }
    }
}