#region

using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public class SSlider : Slider
    {
        public void Bind(UnityAction<float> action)
        {
            onValueChanged.AddListener(action);
        }
    }
}