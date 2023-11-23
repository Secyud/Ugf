using Localization;

namespace Secyud.Ugf.Localization
{
    public class DefaultLocalizer<T>
    {

        private static ILocalizer<T> _localizer;

        public static ILocalizer<T> Localizer =>
            _localizer ??= U.Get<ILocalizer<T, DefaultResource>>();
    }
}