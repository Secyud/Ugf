using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Localization
{
    public class DefaultStringLocalizerFactory:IStringLocalizerFactory,ISingleton
    {
        private readonly Dictionary<Type, Dictionary<string, string>> _localizationStrings = new();
        
        public IDictionary<string, string> GetLocalizerStringDictionary<TResource>()
        {
            if (!_localizationStrings.ContainsKey(typeof(TResource)))
            {
                var path = typeof(TResource).FullName!.Replace('.', '/') ;

                path = $"{path}/{CultureInfo.CurrentCulture.Name}.json";

                using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
                using StreamReader sr = new(fs, Encoding.UTF8);
                
                // _localizationStrings[typeof(TResource)] = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
            }

            return _localizationStrings[typeof(TResource)];
        }
    }
}