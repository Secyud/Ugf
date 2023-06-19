#region

using Localization;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Option;
using System;

#endregion

namespace Secyud.Ugf.Modularity
{
    public class ConfigurationContext : ModuleContextBase
    {
        public IDependencyManager Manager { get; }
        public override IDependencyProvider Provider => Manager;

        private ILocalizerFactory LocalizerFactory =>
            _localizerFactory ??= Manager.Get<ILocalizerFactory>();

        private ILocalizerFactory _localizerFactory;

        public ConfigurationContext(IDependencyManager manager)
        {
            Thrower.IfNull(manager);
            Manager = manager;
        }

        public void AddResource<TResource>()
            where TResource : DefaultResource
        {
            LocalizerFactory.RegisterResource<TResource>();
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
    }
}