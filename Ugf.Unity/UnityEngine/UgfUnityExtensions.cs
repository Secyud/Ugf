#region

using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.ButtonComponents;
using Secyud.Ugf.Localization;
using TMPro;

#endregion

namespace UnityEngine
{
    public static class UgfUnityExtensions
    {
        public static IEnumerable<TButton> SelectVisibleFor<TButton, TItem>(this IEnumerable<TButton> buttons,
            TItem item)
            where TButton : ButtonRegistration<TItem>
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

        public static Vector2 GetMousePosition()
        {
            var x = Input.mousePosition.x / Screen.width * Screen.currentResolution.width;
            var y = (Input.mousePosition.y - Screen.height) / Screen.height * Screen.currentResolution.height;
            
            return new Vector2(x-8, y+8);
        }

        public static string RelativePathTo(this Transform target, Transform root)
        {
            var path = target.name;
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