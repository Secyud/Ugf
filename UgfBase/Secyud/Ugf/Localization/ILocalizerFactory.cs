#region

using Localization;
using System.Collections.Generic;
using System.Globalization;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Localization
{
    public interface ILocalizerFactory<in TObject>:IRegistry
    {
        void AddResource<TResource>() 
            where TResource : DefaultResource;

        void RegisterResource<TResource,TService>() 
            where TResource : DefaultResource
            where TService: ILocalizer<TObject,TResource>;

        IDictionary<string, string> GetDictionary<TResource>()
            where TResource : DefaultResource;

        void ChangeCulture(CultureInfo cultureInfo);
    }
}