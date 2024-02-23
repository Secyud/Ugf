using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Ugf;
using System.Ugf.Collections.Generic;
using Newtonsoft.Json.Linq;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using Secyud.Ugf.VirtualPath;
using UnityEngine.Pool;

namespace Secyud.Ugf.Unity.Localization
{
    public class UgfStringLocalizer : LocalizerBase, IStringLocalizer, IRegistry
    {
        private readonly IVirtualPathManager _virtualPathManager;
        private string _currentCulture = CultureInfo.CurrentCulture.IetfLanguageTag;
        private readonly Dictionary<string, string> _translates = new();
        private readonly SortedDictionary<string, Collection> _resources = new();

        public UgfStringLocalizer(ILocalizerFactory factory, IVirtualPathManager virtualPathManager)
            : base(factory)
        {
            _virtualPathManager = virtualPathManager;
        }

        public void RegisterPath(string virtualPath, string languageName, bool isDefault = false)
        {
            string[] culture = languageName.Split('-');
            string c = culture[0];
            if (!_resources.TryGetValue(c, out Collection collection))
            {
                collection = new Collection();
                _resources[c] = collection;
            }

            if (culture.Length > 1)
            {
                string t = culture[1];
                if (!collection.Specific.TryGetValue(t, out List<string> paths))
                {
                    paths = new List<string>();
                    collection.Specific[t] = paths;
                }

                paths.AddIfNotContains(virtualPath);
                if (isDefault)
                {
                    collection.Default.AddIfNotContains(virtualPath);
                }
            }
            else
            {
                collection.Default.AddIfNotContains(virtualPath);
            }
        }

        public string this[string str] =>
            GetLocalizedString(str);

        public string this[string str, params object[] args] =>
            GetLocalizedString(str, args);

        public override string GetLocalizedString(string str)
        {
            if (str.IsNullOrEmpty()) return "";

            if (!_translates.Any())
                ReloadResources();

            _translates.TryGetValue(str, out string ret);
            return ret ?? str;
        }

        public override string GetLocalizedString(string str, params object[] args)
        {
            return string.Format(GetLocalizedString(str), args);
        }

        public override void ChangeCulture(CultureInfo cultureInfo)
        {
            if (cultureInfo.IetfLanguageTag == _currentCulture)
                return;
            _currentCulture = cultureInfo.IetfLanguageTag;
            ReloadResources();
        }

        public void ReloadResources()
        {
            _translates.Clear();


            string[] culture = _currentCulture.Split('-');

            List<string> paths = ListPool<string>.Get();

            if (!_resources.TryGetValue(culture[0], out Collection collection)) return;
            paths.AddRange(collection.Default);
            if (culture.Length > 1 && collection.Specific
                    .TryGetValue(culture[1], out List<string> list))
            {
                paths.AddRange(list);
            }


            for (int i = 0; i < paths.Count; i++)
            {
                string[] filePaths = _virtualPathManager.GetFilesSingly(paths[i]);
                for (int j = 0; j < filePaths.Length; j++)
                {
                    string filePath = filePaths[j];
                    using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
                    using StreamReader sr = new(fs, Encoding.UTF8);

                    string jsonStr = sr.ReadToEnd();
                    JObject jsonObject = JObject.Parse(jsonStr);
                    foreach ((string key, JToken value) in jsonObject)
                    {
                        _translates[key] = value.Value<string>();
                    }
                }
            }

            ListPool<string>.Release(paths);
        }

        public class Collection
        {
            public List<string> Default { get; } = new();
            public SortedDictionary<string, List<string>> Specific { get; } = new();
        }
    }
}