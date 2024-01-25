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

        /// <summary>
        /// Configure all modules
        /// </summary>
        public void Configure()
        {
            ConfigurationContext context = new(_dependencyManager);

            int moduleCount = Modules.Count;
            for (int i = 0; i < moduleCount; i++)
            {
                if (Modules[i].Instance is IOnPreConfigure onPre)
                {
                    onPre.PreConfigure(context);
                }
            }

            for (int i = 0; i < moduleCount; i++)
            {
                Modules[i].Instance.Configure(context);
            }

            for (int i = 0; i < moduleCount; i++)
            {
                if (Modules[i].Instance is IOnPostConfigure onPost)
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