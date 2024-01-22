using System.Collections.Generic;
using System.Globalization;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Localization
{
    /// <summary>
    /// it works automatically, you don't need to manually call it.
    /// </summary>
    public class UgfLocalizerFactory : ILocalizerFactory, IRegistry
    {
        private readonly List<ILocalizer> _localizers = new();

        public void ChangeCulture(CultureInfo cultureInfo)
        {
            foreach (ILocalizer localizer in _localizers)
            {
                localizer.ChangeCulture(cultureInfo);
            }
        }

        public void RegisterLocalizer(ILocalizer localizer)
        {
            _localizers.Add(localizer);
        }
    }
}