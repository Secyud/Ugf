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
			SText sText = Instantiate(this, parent);
			sText.text = label;
			
			return sText;
		}

		public void SetTextFloatToInt(float f)
		{
			SetText((int)f);
		}
		public void SetText(byte value)
		{
			text = value.ToString();
		}
		public void SetText(int value)
		{
			text = value.ToString();
		}
	}
}