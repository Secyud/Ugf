using System.Globalization;

namespace Secyud.Ugf.Localization
{
    public interface ILocalizer
    {
        string GetLocalizedString(string str);
        string GetLocalizedString(string str,params object[] args);
        void ChangeCulture(CultureInfo cultureInfo);
    }
}