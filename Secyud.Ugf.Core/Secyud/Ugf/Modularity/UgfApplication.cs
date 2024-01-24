using Secyud.Ugf.DependencyInjection;
using System.Collections.Generic;

namespace Secyud.Ugf.Modularity
{
    [Registry(
        typeof(IModuleContainer)
    )]
    public class UgfApplication : IUgfApplication, IRegistry
    {
        private readonly IDependencyManager _dependencyManager;

        public IReadOnlyList<IUgfModuleDescriptor> Modules { get; }

        internal UgfApplication(
            IDependencyManager dependencyManager,
            IReadOnlyList<IUgfModuleDescriptor> modules)
        {
            Throw.IfNull(dependencyManager);
            _dependencyManager = dependencyManager;
            Modules = modules;
        }

        public void Configure()
        {
            ConfigurationContext context = new(_dependencyManager);

            foreach (IUgfModuleDescriptor module in Modules)
            {
                if (module.Instance is IOnPreConfigure onPre)
                {
                    onPre.PreConfigure(context);
                }
            }

            foreach (IUgfModuleDescriptor module in Modules)
            {
                module.Instance.Configure(context);
            }

            foreach (IUgfModuleDescriptor module in Modules)
            {
                if (module.Instance is IOnPostConfigure onPost)
                {
                    onPost.PostConfigure(context);
                }
            }
        }

        /// <summary>
        /// shutdown and exit application
        /// </summary>
        public void Shutdown()
        {
            ShutDownContext context = new(_dependencyManager.CreateScopeProvider());

            for (int i = Modules.Count - 1; i >= 0; i--)
            {
                if (Modules[i].Instance is IOnShutDown module)
                    module.OnShutDown(context);
            }

            UnityEngine.Application.Quit();
        }
    }
}