#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using Secyud.Ugf.VirtualPath;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.Localization
{
    public class UgfStringLocalizer : LocalizerBase, IUgfStringLocalizer,IRegistry
    {
        private readonly IVirtualPathManager _virtualPathManager;
        private string _currentCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        private readonly Dictionary<string, string> _translates = new();
        private readonly List<Tuple<string, string>> _resources = new();

        public UgfStringLocalizer(ILocalizerFactory factory,IVirtualPathManager virtualPathManager)
            : base(factory)
        {
            _virtualPathManager = virtualPathManager;
        }

        public void RegisterPath(string virtualPath, string languageName)
        {
            _resources.Add(new Tuple<string, string>(
                languageName[..2], virtualPath));
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
            if (cultureInfo.TwoLetterISOLanguageName == _currentCulture)
                return;
            _currentCulture = cultureInfo.TwoLetterISOLanguageName;
            ReloadResources();
        }

        public void ReloadResources()
        {
            _translates.Clear();

            List<Tuple<string, string>> strings = new();

            foreach ((string culture, string virtualPath) in _resources)
            {
                if (culture != _currentCulture) continue;
                foreach (string filePath in _virtualPathManager.GetFilesSingly(virtualPath))
                {
                    using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
                    using StreamReader sr = new(fs, Encoding.UTF8);

                    string jsonStr = sr.ReadToEnd();

                    JsonUtility.FromJsonOverwrite(jsonStr,strings);

                    foreach ((string key, string value) in strings)
                    {
                        _translates[key] = value;
                    }
                }
            }
        }
    }
}