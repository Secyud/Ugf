#region

using System.Collections.Generic;
using System.Globalization;

#endregion

namespace Secyud.Ugf.Localization
{
    public interface IStringLocalizerFactory
    {
        void AddResource<TResource>();
        IDictionary<string, string> GetLocalizerStringDictionary<TResource>();

        void ChangeCulture(CultureInfo cultureInfo);
    }
}