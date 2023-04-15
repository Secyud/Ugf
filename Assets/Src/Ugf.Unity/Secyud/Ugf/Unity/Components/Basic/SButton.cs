#region

using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public class SButton : Button
    {
        public void Bind(UnityAction action)
        {
            onClick.AddListener(action);
        }

        public void SetSelect(bool isSelect)
        {
            if (isSelect)
                OnSelect(null);
            else
                OnDeselect(null);
        }
    }
}