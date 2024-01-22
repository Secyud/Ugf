#region

using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Option;
using System;

#endregion

namespace Secyud.Ugf.Modularity
{
    public class ConfigurationContext 
    {
        public IDependencyManager Manager { get; }
        public ConfigurationContext(IDependencyManager manager)
        {
            Throw.IfNull(manager);
            Manager = manager;
        }

        private class OptionConstructor<TOption> : IDependencyConstructor where TOption : new()
        {
            private readonly Action<TOption> _option;

            public OptionConstructor(Action<TOption> option)
            {
                _option = option;
            }

            public object Construct(IDependencyProvider provider)
            {
                Option<TOption> config = new(new TOption());
                _option(config.Value);
                return config;
            }
        }

        public void Configure<TOption>(Action<TOption> option)
            where TOption : new()
        {
            Manager.RegisterCustom<Option<TOption>, IOption<TOption>>(
                new OptionConstructor<TOption>(option)
            );
        }

        public T Get<T>() where T : class
        {
            return Manager.Get<T>();
        }
    }
}