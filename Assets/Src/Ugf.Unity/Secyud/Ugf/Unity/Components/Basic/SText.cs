#region

using TMPro;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    [AddComponentMenu("Secyud/Text", 11)]
    [ExecuteAlways]
    public class SText : TextMeshProUGUI
    {
        public SText Create(Transform parent, string label)
        {
            SText sText = Instantiate(this, parent);
            sText.text = label;
            return sText;
        }
    }
}