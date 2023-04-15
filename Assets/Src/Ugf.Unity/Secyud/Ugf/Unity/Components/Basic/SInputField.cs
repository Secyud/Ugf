#region

using TMPro;
using UnityEngine.Events;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public class SInputField : TMP_InputField
    {
        public void Bind(UnityAction<string> action)
        {
            onValueChanged.AddListener(action);
        }
    }
}