using System;
using System.Collections.Concurrent;

namespace Secyud.Ugf.DependencyInjection
{
    public class CachedDependencyProvider :
        DependencyProviderBase,
        ICachedDependencyProvider,
        IScoped
    {
        private readonly ConcurrentDictionary<Type, object> _cachedServices;
        private readonly DependencyProviderBase _dependencyProvider;

        public CachedDependencyProvider(DependencyProviderBase dependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
            _cachedServices = new ConcurrentDictionary<Type, object>();
            _cachedServices.TryAdd(typeof(IDependencyProvider), _dependencyProvider);
        }

        public override object GetDependency(Type type)
        {
            return _cachedServices.GetOrAdd(type, _dependencyProvider.GetDependency(type));
        }

        public override bool TryGetDependency(Type type, out object dependency)
        {
            if (_cachedServices.ContainsKey(type))
            {
                dependency = _cachedServices[type];
                return true;
            }

            if (!_dependencyProvider.TryGetDependency(type, out dependency))
                return false;

            _cachedServices[type] = dependency;
            return true;
        }


        internal override DependencyDescriptor GetDescriptor(Type type)
        {
            return _dependencyProvider.GetDescriptor(type);
        }
    }
}