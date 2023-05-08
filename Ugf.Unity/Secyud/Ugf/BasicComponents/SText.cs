#region

using TMPro;
using UnityEngine;

#endregion

namespace Secyud.Ugf.BasicComponents
{
    public class SText : TextMeshProUGUI
    {
        public SText Create(Transform parent, string label)
        {
            var sText = Instantiate(this, parent);
            sText.text = label;
            return sText;
        }

        public void SetTextFloatToInt(float f)
        {
            text = ((int)f).ToString();
        }
    }
}