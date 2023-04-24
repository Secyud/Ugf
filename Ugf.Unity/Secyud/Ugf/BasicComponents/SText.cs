#region

using TMPro;
using UnityEngine;

#endregion

namespace Secyud.Ugf.BasicComponents
{
    [AddComponentMenu("Secyud/Text", 11)]
    [ExecuteAlways]
    public class SText : TextMeshProUGUI
    {
        public SText Create(Transform parent, string label)
        {
            var sText = Instantiate(this, parent);
            sText.text = label;
            return sText;
        }
    }
}