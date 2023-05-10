#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.BasicComponents
{
	public class SImage : Image
	{
		public virtual Sprite Sprite
		{
			get => sprite;
			set
			{
				sprite = value;
				enabled = sprite;
			}
		}

		public void SetRotation(float value)
		{
			rectTransform.rotation = Quaternion.Euler(0, 0, value);
		}

		public void SetCollapse(bool value)
		{
			rectTransform.rotation = Quaternion.Euler(0, 0, value ? 0 : -90);
		}
	}
}