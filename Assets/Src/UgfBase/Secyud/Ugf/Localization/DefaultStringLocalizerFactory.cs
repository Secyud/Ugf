#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Localization
{
    public class DefaultStringLocalizerFactory : IStringLocalizerFactory, ISingleton
    {
        private readonly Dictionary<Type, Dictionary<string, string>> _localizationStrings = new();
        private readonly Dictionary<Type, List<Type>> _registeredResource = new();

        public void AddResource<TResource>()
        {
            AddResource(typeof(TResource));
        }

        private void AddResource([NotNull] Type resourceType)
        {
            Type toResource =
                resourceType
                    .GetCustomAttribute<ResourceNameAttribute>()?
                    .ToResource ?? resourceType;

            if (!_registeredResource.ContainsKey(toResource))
                _registeredResource[toResource] = new();

            _registeredResource[toResource].AddIfNotContains(resourceType);
        }

        public IDictionary<string, string> GetLocalizerStringDictionary<TResource>()
        {
            return GetLocalizerStringDictionary(typeof(TResource));
        }

        private IDictionary<string, string> GetLocalizerStringDictionary(Type resource)
        {
            if (!_localizationStrings.ContainsKey(resource))
            {
                if (!_registeredResource.ContainsKey(resource))
                    throw new UgfException("Please use basic resource registered!");

                _localizationStrings[resource] = new();

                List<Type> resources = _registeredResource[resource];

                foreach (Type addedResource in resources)
                {
                    CreateLocalizationStrings(addedResource, _localizationStrings[resource]);
                }
            }

            return _localizationStrings[resource];
        }

        public void ChangeCulture(CultureInfo cultureInfo)
        {
            _localizationStrings.Clear();
            CultureInfo.CurrentCulture = cultureInfo;
        }

        private static void CreateLocalizationStrings(Type resourceType, IDictionary<string, string> localizer)
        {
            string path = resourceType.FullName!.Replace('.', '/');

            path = Path.Combine(UgfConsts.AppPath, $"{path}/{CultureInfo.CurrentCulture.Name}.json");

            if (!File.Exists(path))
                return;

            using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
            using StreamReader sr = new(fs, Encoding.UTF8);

            string jsonStr = sr.ReadToEnd();

            Dictionary<string, string> addedWords = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);

            foreach (KeyValuePair<string, string> word in addedWords)
                localizer[word.Key] = word.Value;
        }
    }
}