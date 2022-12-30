using UnityEngine.Events;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.Ui
{
    public class SToggle:Toggle
    {
        public void Bind(UnityAction<bool> action)
        {
            onValueChanged.AddListener(action);
        }
    }
}