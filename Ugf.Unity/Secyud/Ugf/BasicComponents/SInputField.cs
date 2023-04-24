#region

using TMPro;
using UnityEngine.Events;

#endregion

namespace Secyud.Ugf.BasicComponents
{
    public class SInputField : TMP_InputField
    {
        public void Bind(UnityAction<string> action)
        {
            Clear();
            onValueChanged.AddListener(action);
        }

        private void Clear()
        {
            onValueChanged.RemoveAllListeners();
        }
    }
}