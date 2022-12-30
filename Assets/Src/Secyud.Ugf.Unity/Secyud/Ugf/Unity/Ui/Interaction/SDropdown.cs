using TMPro;
using UnityEngine.Events;

namespace Secyud.Ugf.Unity.Ui
{
    public class SDropdown:TMP_Dropdown
    {
        public  void Bind(UnityAction<int> action)
        {
            onValueChanged.AddListener(action);
        }
    }
}