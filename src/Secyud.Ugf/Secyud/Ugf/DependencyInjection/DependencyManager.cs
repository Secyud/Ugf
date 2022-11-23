using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Secyud.Ugf.DependencyInjection;

public class DependencyManager :
    DependencyProviderBase,
    IDependencyManager
{
    private readonly IDependencyCollection _dependencyCollection;

    private readonly ConcurrentDictionary<Type, object> _singletonInstances;

    internal DependencyManager(IDependencyCollection dependencyCollection = null)
    {
        _dependencyCollection = dependencyCollection ?? new DependencyCollection();
        _singletonInstances = new ConcurrentDictionary<Type, object>
        {
            [GetType()] = this
        };
        AddSelf<IDependencyManager>();
        AddSelf<IDependencyProvider>();
        AddSelf<IDependencyRegistrar>();
        AddSelf<IDependencyScopeFactory>();
    }

    public override object GetDependency(Type type)
    {
        var descriptor = GetDescriptor(type);

        if (descriptor is null)
            throw new UgfException($"Could not find dependency: {type.FullName}!");

        if (descriptor.DependencyLifeTime == DependencyLifeTime.Transient)
            return descriptor.CreateInstance(this);

        if (!_singletonInstances.ContainsKey(descriptor.ImplementationType))
            _singletonInstances[descriptor.ImplementationType] = descriptor.CreateInstance(this);
        return _singletonInstances[descriptor.ImplementationType];
    }

    public override bool TryGetDependency(Type type, out object dependency)
    {
        var descriptor = GetDescriptor(type);

        if (descriptor is null)
        {
            dependency = default;
            return false;
        }

        if (descriptor.DependencyLifeTime == DependencyLifeTime.Transient)
        {
            dependency = descriptor.CreateInstance(this);
        }
        else
        {
            if (!_singletonInstances.ContainsKey(descriptor.ImplementationType))
                _singletonInstances[descriptor.ImplementationType] = descriptor.CreateInstance(this);
            dependency = _singletonInstances[descriptor.ImplementationType];
        }

        return true;
    }

    public IDependencyScope CreateScope()
    {
        return new DependencyScope(new DependencyProvider(this));
    }

    public void AddAssembly(Assembly assembly)
    {
        AddTypes(
            assembly.GetTypes()
                .Where(type =>
                    type is
                    {
                        IsClass: true,
                        IsAbstract: false,
                        IsGenericType: false
                    })
        );
    }

    public void AddTypes(IEnumerable<Type> types)
    {
        foreach (var type in types)
            AddType(type);
    }

    public void AddType(Type type)
    {
        if (IsConventionalRegistrationDisabled(type))
            return;

        var lifeTime = GetLifeTimeOrNull(type);

        if (lifeTime == null)
            return;

        var exposedServiceTypes = ExposedServiceExplorer.GetExposedServices(type);

        foreach (var exposedServiceType in exposedServiceTypes)
            CreateDependencyDescriptor(
                type,
                exposedServiceType,
                lifeTime.Value
            );
    }

    public void AddType<T>()
    {
        AddType(typeof(T));
    }

    public void AddSingleton<TExposed>(TExposed instance)
    {
        AddSingleton(typeof(TExposed), instance);
    }

    public void AddSingleton(Type type, object instance)
    {
        CreateDependencyDescriptor(
            instance.GetType(),
            type,
            DependencyLifeTime.Singleton);
        _singletonInstances[instance.GetType()] = instance;
    }

    public void AddSingleton<T, TExposed>()
    {
        CreateDependencyDescriptor(
            typeof(T),
            typeof(TExposed),
            DependencyLifeTime.Singleton);
    }

    public void AddScoped<T, TExposed>()
    {
        CreateDependencyDescriptor(
            typeof(T),
            typeof(TExposed),
            DependencyLifeTime.Scoped);
    }

    public void AddTransient<T, TExposed>()
    {
        CreateDependencyDescriptor(
            typeof(T),
            typeof(TExposed),
            DependencyLifeTime.Transient);
    }

    private void AddSelf<TExposed>()
    {
        CreateDependencyDescriptor(GetType(), typeof(TExposed), DependencyLifeTime.Singleton);
    }

    internal override DependencyDescriptor GetDescriptor(Type type)
    {
        return !_dependencyCollection.ContainsKey(type) ? null : _dependencyCollection[type];
    }

    private bool IsConventionalRegistrationDisabled(Type type)
    {
        return type.IsDefined(typeof(DisableRegistrationAttribute), true);
    }

    private static DependencyLifeTime? GetLifeTimeOrNull(Type type)
    {
        if (typeof(ITransient).GetTypeInfo().IsAssignableFrom(type))
            return DependencyLifeTime.Transient;

        if (typeof(IScoped).GetTypeInfo().IsAssignableFrom(type))
            return DependencyLifeTime.Scoped;

        if (typeof(ISingleton).GetTypeInfo().IsAssignableFrom(type))
            return DependencyLifeTime.Singleton;

        return null;
    }

    private void CreateDependencyDescriptor(
        Type implementationType,
        Type exposedType,
        DependencyLifeTime lifeTime)
    {
        _dependencyCollection[exposedType]
            = DependencyDescriptor.Describe(
                implementationType,
                lifeTime
            );
    }
}