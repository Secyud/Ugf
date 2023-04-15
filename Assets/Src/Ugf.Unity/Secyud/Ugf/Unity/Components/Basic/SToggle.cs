#region

using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public class SToggle : Toggle
    {
        public void Bind(UnityAction<bool> action)
        {
            onValueChanged.AddListener(action);
        }
    }
}