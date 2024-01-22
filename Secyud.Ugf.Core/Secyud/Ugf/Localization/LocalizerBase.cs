using System.Globalization;

namespace Secyud.Ugf.Localization
{
    public abstract class LocalizerBase:ILocalizer
    {
        protected LocalizerBase(ILocalizerFactory factory)
        {
            factory.RegisterLocalizer(this);
        }

        public abstract string GetLocalizedString(string str);
        public abstract string GetLocalizedString(string str, params object[] args);
        public abstract void ChangeCulture(CultureInfo cultureInfo);
    }
}