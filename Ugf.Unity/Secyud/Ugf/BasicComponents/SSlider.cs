#region

using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.BasicComponents
{
    public class SSlider : Slider
    {
        public void Bind(UnityAction<float> action)
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