using System.Collections.Generic;
using System.Linq;

namespace Secyud.Ugf.Abstraction
{
    public static class FunctionExtension
    {
        public static void InvokeList<TActionable, TTarget>(this IEnumerable<TActionable> actions, TTarget target)
            where TActionable : class, IActionable<TTarget>
        {
            foreach (TActionable action in actions.OrderBy(GetPriority))
            {
                action.Invoke(target);
            }
        }

        public static void InvokeList<TActionable>(this IEnumerable<TActionable> actions)
            where TActionable : class, IActionable
        {
            foreach (TActionable action in actions.OrderBy(GetPriority))
            {
                action.Invoke();
            }
        }

        private static int GetPriority(object obj)
        {
            if (obj is IHasPriority hasPriority)
            {
                return hasPriority.Priority;
            }

            return 0;
        }

        public static void InstallList<TInstallable, TTarget>(this IEnumerable<TInstallable> installables,
            TTarget target)
            where TInstallable : class, IInstallable<TTarget>
        {
            foreach (TInstallable installable in installables)
            {
                installable.InstallOn(target);
            }
        }

        public static void InstallList<TInstallable>(this IEnumerable<TInstallable> installables)
            where TInstallable : class, IInstallable
        {
            foreach (TInstallable installable in installables)
            {
                installable.Install();
            }
        }

        public static void UninstallList<TInstallable, TTarget>(this IEnumerable<TInstallable> installables,
            TTarget target)
            where TInstallable : class, IInstallable<TTarget>
        {
            foreach (TInstallable installable in installables)
            {
                installable.UninstallFrom(target);
            }
        }

        public static void UninstallList<TInstallable>(this IEnumerable<TInstallable> installables)
            where TInstallable : class, IInstallable
        {
            foreach (TInstallable installable in installables)
            {
                installable.Uninstall();
            }
        }
    }
}