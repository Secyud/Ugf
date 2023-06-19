using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity;

public abstract class ModuleContextBase : IDisposable
{
    private SortedDictionary<Type, IDisposable> Configs { get; } = new();

    public abstract IDependencyProvider Provider { get; }

    public void SetConfig(
        [NotNull] IDisposable option)
    {
        Type optionType = option.GetType();
        if (Configs.TryGetValue(optionType, out IDisposable o))
            o.Dispose();
        Configs[option.GetType()] = option;
    }

    public TConfig GetConfig<TConfig>() where TConfig : class
    {
        return Configs[typeof(TConfig)] as TConfig;
    }

    public T Get<T>() where T : class
    {
        return Provider.Get<T>();
    }

    public virtual void Dispose()
    {
        foreach (IDisposable config in Configs.Values)
            config.Dispose();
    }
}