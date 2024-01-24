using System.Collections.Generic;
using System.Globalization;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Localization
{
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

        /// <summary>
        /// It works automatically in <see cref="LocalizerBase"/>,
        /// you don't need to call it manually.
        /// </summary>
        /// <param name="localizer"></param>
        public void RegisterLocalizer(ILocalizer localizer)
        {
            _localizers.Add(localizer);
        }
    }
}