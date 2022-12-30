using System.Collections.Generic;

namespace Secyud.Ugf.Localization
{
    public class DefaultStringLocalizer<TResource>:IStringLocalizer<TResource>
    {
        private readonly IDictionary<string,string> _dictionary;
        
        public DefaultStringLocalizer(IStringLocalizerFactory stringLocalizerFactory)
        {
            _dictionary = stringLocalizerFactory.GetLocalizerStringDictionary<TResource>();
        }

        public string this[string str] => _dictionary.ContainsKey(str) ? _dictionary[str] : str;

        public string this[string str, params object[] args] => string.Format(str,args);
    }
}