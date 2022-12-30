using UnityEngine.Events;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.Ui
{
    public class SSlider : Slider
    {
        public void Bind(UnityAction<float> action)
        {
            onValueChanged.AddListener(action);
        }
    }
}