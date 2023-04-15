#region

using TMPro;
using UnityEngine.Events;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public class SDropdown : TMP_Dropdown
    {
        public void Bind(UnityAction<int> action)
        {
            onValueChanged.AddListener(action);
        }
    }
}