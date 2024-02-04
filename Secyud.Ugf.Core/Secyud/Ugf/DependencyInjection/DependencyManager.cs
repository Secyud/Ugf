using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Secyud.Ugf.DataManager;

namespace Secyud.Ugf.DependencyInjection
{
    [Registry(
        typeof(IDependencyProvider),
        typeof(IDependencyRegistrar)
    )]
    public class DependencyManager :
        DependencyProviderBase,
        IDependencyManager
    {
        internal static DependencyManager Instance { get; private set; }

        private readonly IDependencyCollection _dependencyDescriptors;
        private readonly ConcurrentDictionary<Type, DependencyScopeProvider> _scopes = new();

        internal DependencyManager(IDependencyCollection dependencyCollection = null)
        {
            Instance = this;
            _dependencyDescriptors = dependencyCollection ?? new DependencyCollection();
            RegisterInstance<IScopeManager>(this);
            RegisterInstance<IDependencyProvider>(this);
        }

        public override DependencyDescriptor GetDependencyDescriptor(Type exposedType)
        {
            if (_dependencyDescriptors.TryGetValue(exposedType, out var descriptor))
            {
                return descriptor;
            }

            throw new UgfException($"Cannot get service not registered: {exposedType}");
        }

        public void AddAssembly(Assembly assembly)
        {
            AddTypes(
                assembly.GetTypes().Where(type =>
                    type is
                    {
                        IsPublic: true,
                        IsClass: true,
                        IsAbstract: false,
                        IsGenericType: false
                    }).ToArray());
        }

        public void AddTypes(params Type[] types)
        {
            foreach (Type type in types)
            {
                AddType(type);
            }
        }

        public void AddType<T>()
        {
            AddType(typeof(T));
        }

        public void AddType(Type type)
        {
            if (IsRegistryDisabled(type))
                return;


            if (typeof(IRegistry).IsAssignableFrom(type))
            {
                RegistryAttribute registryAttr = type.GetCustomAttribute<RegistryAttribute>(true)
                                                 ?? RegistryAttribute.Singleton;

                var descriptor = CreateDirectDescriptor(type, registryAttr);
                
                List<Type> exposedServiceTypes = ExposedServiceExplorer.GetExposedServices(type);

                foreach (Type exposedServiceType in exposedServiceTypes)
                    _dependencyDescriptors[exposedServiceType] = descriptor;
                TypeManager.Instance.AddType(type, true);
            }
            else
            {
                TypeManager.Instance.AddType(type, false);
            }
        }

        private DependencyDescriptor CreateDirectDescriptor(
            Type implementationType,
            RegistryAttribute registryAttribute)
        {
            return DependencyDescriptor.Describe(implementationType, this,
                new DependencyConstructor(implementationType), registryAttribute);
        }

        protected override void HandleScope(DependencyDescriptor dd, InstanceDescriptor id)
        {
            id.ObjectAccessor = () => _scopes[dd.RegistryAttribute.DependScope].Get(dd.ImplementationType);
        }

        public void RegisterInstance<TExposed>(TExposed instance)
        {
            RegisterInstance(typeof(TExposed), instance);
        }

        public void RegisterInstance(Type type, object instance)
        {
            var descriptor = CreateDirectDescriptor(
                instance.GetType(), RegistryAttribute.Singleton);
            descriptor.Instance = instance;
            _dependencyDescriptors[type] = descriptor;
        }

        public void Register<T, TExposed>(DependencyLifeTime lifeTime = DependencyLifeTime.Singleton)
            where T : TExposed
        {
            var descriptor = CreateDirectDescriptor(
                typeof(T), RegistryAttribute.Singleton);
            _dependencyDescriptors[typeof(TExposed)] = descriptor;
        }

        public void RegisterCustom<T, TExposed>(IDependencyConstructor constructor,
            DependencyLifeTime lifeTime = DependencyLifeTime.Singleton)
            where T : TExposed
        {
            _dependencyDescriptors[typeof(TExposed)] = DependencyDescriptor.Describe(
                typeof(T), this, constructor,
                new RegistryAttribute { LifeTime = lifeTime });
        }

        public IDependencyProvider CreateScopeProvider()
        {
            return new DependencyScopeProvider()
            {
                ParentProvider = this
            };
        }

        private bool IsRegistryDisabled(Type type)
        {
            return type.IsDefined(typeof(RegistryDisabledAttribute));
        }

        public TScope CreateScope<TScope>() where TScope : DependencyScopeProvider
        {
            return CreateScope(typeof(TScope)) as TScope;
        }

        public DependencyScopeProvider CreateScope(Type scopeType)
        {
            if (!_scopes.TryGetValue(scopeType, out DependencyScopeProvider scope))
            {
                DependencyDescriptor dd = GetDependencyDescriptor(scopeType);
                DependencyScopeProvider pScope = null;
                Type dependScope = dd.RegistryAttribute.DependScope;
                if (dependScope is not null)
                    if (!_scopes.TryGetValue(dependScope, out pScope))
                        throw new UgfException("Depend scope is not available!");

                scope = Get(scopeType) as DependencyScopeProvider;

                _scopes[scopeType] = scope ?? throw new UgfException($"Cannot get scope: {scopeType}");

                if (pScope is null)
                    scope.ParentProvider = this;
                else
                {
                    scope.ParentProvider = pScope;
                    pScope.SubProviders.Add(scope);
                }

                scope.OnInitialize();
            }

            return scope;
        }

        public void DestroyScope<TScope>() where TScope : DependencyScopeProvider
        {
            DestroyScope(typeof(TScope));
        }

        private void DestroyScope(Type scopeType)
        {
            if (!_scopes.TryRemove(scopeType, out DependencyScopeProvider scopeDescriptor))
            {
                return;
            }

            foreach (DependencyScopeProvider scope in scopeDescriptor.SubProviders)
            {
                DestroyScope(scope.GetType());
            }

            scopeDescriptor.InstanceDescriptor.Clear();
            scopeDescriptor.Dispose();
        }

        public TScope GetScope<TScope>() where TScope : DependencyScopeProvider
        {
            if (!_scopes.TryGetValue(typeof(TScope), out DependencyScopeProvider scope))
                throw new UgfException($"Cannot get scope {typeof(TScope)} this time. Please ensure it is exist.");
            return scope as TScope;
        }
    }
}