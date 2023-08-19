using UnityEngine.Events;
using UnityEngine.UI;

namespace Secyud.Ugf.BasicComponents
{
    public class SToggle:Toggle
    {
        public void Bind(UnityAction<bool> action)
        {
            Clear();
            onValueChanged.AddListener(action);
        }
		
        public void Clear()
        {
            onValueChanged.RemoveAllListeners();
        }
    }
}