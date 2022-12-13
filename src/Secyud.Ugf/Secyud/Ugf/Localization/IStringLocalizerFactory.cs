using System.Collections.Generic;

namespace Secyud.Ugf.Localization
{
    public interface IStringLocalizerFactory
    {
        IDictionary<string, string> GetLocalizerStringDictionary<TResource>();
    }
}