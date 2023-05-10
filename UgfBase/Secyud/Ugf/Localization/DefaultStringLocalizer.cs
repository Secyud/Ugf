#region

using Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace Secyud.Ugf.Localization
{
	public class DefaultStringLocalizer<TResource> : IStringLocalizer<TResource>
		where TResource : DefaultResource
	{
		private readonly IDictionary<string, string> _dictionary;

		public DefaultStringLocalizer(ILocalizerFactory localizerFactory)
		{
			_dictionary = localizerFactory.GetLocalizerStringDictionary<TResource>();
		}

		public string this[string str]
		{
			get
			{
				if (str.IsNullOrEmpty())
					return "";

				_dictionary.TryGetValue(str, out string ret);
				return ret ?? str;
			}
		}

		public string this[string str, params object[] args] => string.Format(this[str], args);

		public string FormatTranslate(string str)
		{
			string[] strList = str.Split(',');
			object[] paramList = strList.Length > 1 ? strList[1..]
				.Select(s => s.StartsWith('&') ? this[s[1..]] : s)
				.ToArray<object>() : Array.Empty<object>();

			return this[strList[0], paramList];
		}

		public string Translate(string str)
		{
			return Regex.Replace(str, "\\[[^\\[\\]]*\\]", s => FormatTranslate(s.Value[1..^1]));
		}
	}
}