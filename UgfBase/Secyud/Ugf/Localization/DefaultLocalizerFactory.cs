#region

using Localization;
using Secyud.Ugf.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Ugf.Collections.Generic;

#endregion

namespace Secyud.Ugf.Localization
{
    public class DefaultLocalizerFactory<TObject> : ILocalizerFactory<TObject>
    {
        private readonly IDependencyRegistrar _registrar;
        private readonly Dictionary<Type, LocalizeDescriptor> _localizeDescriptors = new();

        private LocalizeDescriptor GetDescriptor(Type type)
        {
            if (!_localizeDescriptors.TryGetValue(type, out LocalizeDescriptor descriptor))
            {
                descriptor = new LocalizeDescriptor();
                descriptor.Resources.AddIfNotContains(type);
                _localizeDescriptors[type] = descriptor;
            }

            return descriptor;
        }
        
        public DefaultLocalizerFactory(IDependencyRegistrar registrar)
        {
            _registrar = registrar;
        }

        public void AddResource<TResource>()
            where TResource : DefaultResource
        {
            AddResource(typeof(TResource));
        }
        
        private void AddResource([NotNull] Type resourceType)
        {
            Type toResource = resourceType
                .GetCustomAttribute<ResourceNameAttribute>()?
                .ToResource ?? resourceType;
            LocalizeDescriptor descriptor = GetDescriptor(toResource);
            descriptor.Resources.AddIfNotContains(resourceType);
        }

        public void RegisterResource<TResource,TService>()
            where TResource : DefaultResource 
            where TService : ILocalizer<TObject, TResource>
        {
            _registrar.Register<
                TService,
                ILocalizer<TObject,TResource>>(
                DependencyLifeTime.Transient);
            AddResource<TResource>();
        }

        public IDictionary<string, string> GetDictionary<TResource>()
            where TResource : DefaultResource
        {
            return GetDescriptor(typeof(TResource)).Dictionary;
        }

        public void ChangeCulture(CultureInfo cultureInfo)
        {
            _localizeDescriptors.Clear();
            CultureInfo.CurrentCulture = cultureInfo;
        }
    }
}