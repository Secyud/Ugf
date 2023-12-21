using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Secyud.Ugf.VirtualPath;

namespace Secyud.Ugf.Localization
{
    public class LocalizeDescriptor
    {
        private Dictionary<string, string> _dictionary;
        public List<Type> Resources { get; } = new();

        public Dictionary<string, string> Dictionary => _dictionary ??= CreateDictionary();

        private static string[] LocalizationPath(Type resource)
        {
            return U.Get<IVirtualPathManager>()
                .GetFilesSingly($"Localization/{resource.Name}/{CultureInfo.CurrentCulture.Name}.json");
        }

        private Dictionary<string, string> CreateDictionary()
        {
            Dictionary<string, string> ret = new();
            foreach (string path in Resources.SelectMany(LocalizationPath))
            {
                using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
                using StreamReader sr = new(fs, Encoding.UTF8);

                string jsonStr = sr.ReadToEnd();

                Dictionary<string, string> addedWords =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);

                foreach ((string key, string value) in addedWords)
                {
                    ret[key] = value;
                }
            }

            return ret;
        }
    }
}