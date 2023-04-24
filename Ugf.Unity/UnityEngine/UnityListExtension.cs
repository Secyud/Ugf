#region

using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.ButtonComponents;

#endregion

namespace UnityEngine
{
    public static class UnityListExtension
    {
        public static IEnumerable<TButton> SelectVisibleFor<TButton, TItem>(this IEnumerable<TButton> buttons,
            TItem item)
            where TButton : ButtonRegistration<TItem>
        {
            return buttons.Where(u => u.Visible(item));
        }
    }
}