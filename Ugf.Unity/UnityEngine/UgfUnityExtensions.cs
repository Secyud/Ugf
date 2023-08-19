#region

using Secyud.Ugf.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.TableComponents.ButtonComponents;
using TMPro;

#endregion

namespace UnityEngine
{
	public static class UgfUnityExtensions
	{
		public static IEnumerable<TButton> SelectVisibleFor<TButton, TItem>(this IEnumerable<TButton> buttons,
			TItem item)
			where TButton : ButtonDescriptor<TItem>
		{
			return buttons.Where(u => u.Visible(item));
		}

		public static List<TMP_Dropdown.OptionData> GetOptionsFromEnum(this IStringLocalizer l, Type enumType,
			string prefix = null, string suffix = null)
		{
			return Enum
				.GetNames(enumType)
				.Select(u => new TMP_Dropdown.OptionData(l[$"{prefix}{u}{suffix}"]))
				.ToList();
		}

		public static Vector2 GetMousePosition(Vector2 bias = default, bool useDefault = true)
		{
			float x = Input.mousePosition.x / Screen.width * Screen.currentResolution.width;
			float y = Input.mousePosition.y / Screen.height * Screen.currentResolution.height -
				Screen.currentResolution.height;

			return new Vector2(x, y) + (useDefault ? new Vector2(-8, 8) : bias);
		}

		public static string RelativePathTo(this Transform target, Transform root)
		{
			string path = target.name;
			try
			{
				while (target.parent != root)
				{
					target = target.parent;
					path = target.name + "/" + path;
				}
			}
			catch
			{
				path = string.Empty;
			}

			return path;
		}
	}
}