#region

using TMPro;
using UnityEngine.Events;

#endregion

namespace Secyud.Ugf.BasicComponents
{
    public class SDropdown : TMP_Dropdown
    {
        public void Bind(UnityAction<int> action)
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