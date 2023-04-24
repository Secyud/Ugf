#region

using System;
using System.Collections.Generic;
using Localization;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Option;

#endregion

namespace Secyud.Ugf.Modularity
{
    public class ConfigurationContext
    {
        public ConfigurationContext(IDependencyManager manager)
        {
            Thrower.IfNull(manager);
            Manager = manager;
            Items = new Dictionary<string, object>();
        }

        public IDependencyManager Manager { get; }

        private IDictionary<string, object> Items { get; }

        public object this[string key]
        {
            get => Items.GetOrDefault(key);
            set => Items[key] = value;
        }

        public T Get<T>() where T : class
        {
            return Manager.Get<T>();
        }

        public void AddResource<TResource>()
            where TResource : DefaultResource
        {
            Manager.AddTransient<DefaultStringLocalizer<TResource>, IStringLocalizer<TResource>>();
            Manager.Get<IStringLocalizerFactory>().AddResource<TResource>();
        }

        public void Configure<TOption>(Action<TOption> option)
            where TOption : new()
        {
            Manager.AddCustom<Option<TOption>, IOption<TOption>>(
                () =>
                {
                    var config = new Option<TOption>(new TOption());
                    option(config.Value);
                    return config;
                });
        }
    }
}