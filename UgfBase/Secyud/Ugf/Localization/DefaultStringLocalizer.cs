#region

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Localization;

#endregion

namespace Secyud.Ugf.Localization
{
    public class DefaultStringLocalizer<TResource> : IStringLocalizer<TResource>
        where TResource : DefaultResource
    {
        private readonly IDictionary<string, string> _dictionary;

        public DefaultStringLocalizer(IStringLocalizerFactory stringLocalizerFactory)
        {
            _dictionary = stringLocalizerFactory.GetLocalizerStringDictionary<TResource>();
        }

        public string this[string str] => str is null ? "" : _dictionary.ContainsKey(str) ? _dictionary[str] : str;

        public string this[string str, params object[] args] => string.Format(this[str], args);

        public string FormatTranslate(string str)
        {
            var strList = str.Split(',');

            var paramList =
                strList[1..]
                    .Select(s => s.StartsWith('&') ? this[s[1..]] : s)
                    .ToArray<object>();

            return this[strList[0], paramList];
        }

        public string Translate(string str)
        {
            return Regex.Replace(str, "\\[[^\\[\\]]*\\]", s => FormatTranslate(s.Value[1..^1]));
        }
    }
}