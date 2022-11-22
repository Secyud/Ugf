using System;
using System.Reflection;
using System.Threading.Tasks;

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
        public virtual Task OnInitializationAsync(InitializationContext context)
        {
            OnInitialization(context);
            return Task.CompletedTask;
        }

        public virtual Task OnPostInitializationAsync(InitializationContext context)
        {
            OnPostInitialization(context);
            return Task.CompletedTask;
        }

        public virtual Task OnPreInitializationAsync(InitializationContext context)
        {
            OnPreInitialization(context);
            return Task.CompletedTask;
        }

        public virtual Task OnShutdownAsync(ShutdownContext context)
        {
            OnShutdown(context);
            return Task.CompletedTask;
        }

        public virtual Task PostConfigureAsync(ConfigurationContext context)
        {
            PostConfigure(context);
            return Task.CompletedTask;
        }

        public virtual Task PreConfigureAsync(ConfigurationContext context)
        {
            PreConfigure(context);
            return Task.CompletedTask;
        }

        public virtual Task ConfigureAsync(ConfigurationContext context)
        {
            Configure(context);
            return Task.CompletedTask;
        }

        public virtual void Configure(ConfigurationContext context)
        {
        }

        public virtual void PreConfigure(ConfigurationContext context)
        {
        }

        public virtual void PostConfigure(ConfigurationContext context)
        {
        }

        public virtual void OnPreInitialization(InitializationContext context)
        {
        }

        public virtual void OnInitialization(InitializationContext context)
        {
        }

        public virtual void OnPostInitialization(InitializationContext context)
        {
        }

        public virtual void OnShutdown(ShutdownContext context)
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