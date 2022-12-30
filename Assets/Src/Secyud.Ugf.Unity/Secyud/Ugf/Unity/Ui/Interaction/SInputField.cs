using TMPro;
using UnityEngine.Events;

namespace Secyud.Ugf.Unity.Ui
{
    public class SInputField:TMP_InputField
    {
        public void Bind( UnityAction<string> action)
        {
            onValueChanged.AddListener(action);
        }
    }
}