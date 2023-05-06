#region

using Localization;
using System.Collections.Generic;
using System.Globalization;

#endregion

namespace Secyud.Ugf.Localization
{
    public interface IStringLocalizerFactory
    {
        void AddResource<TResource>()where TResource : DefaultResource;
        void RegisterResource<TResource>()where TResource : DefaultResource;
        IDictionary<string, string> GetLocalizerStringDictionary<TResource>() where TResource : DefaultResource;
        void ChangeCulture(CultureInfo cultureInfo);
    }
}