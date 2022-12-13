using System;
using System.Reflection;
using System.Threading.Tasks;
using Secyud.Ugf.Localization;
using Secyud.Ugf.Option;
using Secyud.Ugf.Option.Abstraction;

namespace Secyud.Ugf.Modularity
{
    public abstract class UgfModule :
        IUgfModule,
        IOnPreInitialization,
        IOnInitialization,
        IOnPostInitialization,
        IOnShutdown,
        IPreConfigure,
        IPostConfigure
    {
        internal ConfigurationContext ConfigurationContext;

        protected void AddResource<TResource>()
        {
            ConfigurationContext.Manager
                .AddTransient<DefaultStringLocalizer<TResource>,IStringLocalizer<TResource>>();
        }
        
        protected void Configure<TOption>(Action<TOption> option) 
            where TOption : new()
        {
            ConfigurationContext.Manager.AddCustom<Option<TOption>,IOption<TOption>>(
                () =>
                {
                    var config = new Option<TOption>(new());
                    option(config.Value);
                    return config;
                });
        }
        
        public virtual Task OnGameInitializationAsync(InitializationContext context)
        {
            OnGameInitialization(context);
            return Task.CompletedTask;
        }

        public virtual Task OnGamePostInitializationAsync(InitializationContext context)
        {
            OnGamePostInitialization(context);
            return Task.CompletedTask;
        }

        public virtual Task OnGamePreInitializationAsync(InitializationContext context)
        {
            OnGamePreInitialization(context);
            return Task.CompletedTask;
        }

        public virtual Task OnGameShutdownAsync(ShutdownContext context)
        {
            OnGameShutdown(context);
            return Task.CompletedTask;
        }

        public virtual Task PostConfigureGameAsync(ConfigurationContext context)
        {
            PostConfigureGame(context);
            return Task.CompletedTask;
        }

        public virtual Task PreConfigureGameAsync(ConfigurationContext context)
        {
            PreConfigureGame(context);
            return Task.CompletedTask;
        }

        public virtual Task ConfigureGameAsync(ConfigurationContext context)
        {
            ConfigureGame(context);
            return Task.CompletedTask;
        }

        public virtual void ConfigureGame(ConfigurationContext context)
        {
        }

        protected virtual void PreConfigureGame(ConfigurationContext context)
        {
        }

        protected virtual void PostConfigureGame(ConfigurationContext context)
        {
        }

        protected virtual void OnGamePreInitialization(InitializationContext context)
        {
        }

        protected virtual void OnGameInitialization(InitializationContext context)
        {
        }

        protected virtual void OnGamePostInitialization(InitializationContext context)
        {
        }

        protected virtual void OnGameShutdown(ShutdownContext context)
        {
        }

        public static bool IsUgfModule(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return
                typeInfo.IsClass &&
                !typeInfo.IsAbstract &&
                !typeInfo.IsGenericType &&
                typeof(IUgfModule).GetTypeInfo().IsAssignableFrom(type);
        }

        internal static void CheckUgfModuleType(Type moduleType)
        {
            if (!IsUgfModule(moduleType))
                throw new ArgumentException("Given type is not an UGF module: " + moduleType.AssemblyQualifiedName);
        }
    }
}