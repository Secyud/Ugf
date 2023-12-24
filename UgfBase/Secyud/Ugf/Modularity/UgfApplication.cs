#region

using Secyud.Ugf.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Ugf.Collections.Generic;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.DataManager;

#endregion

namespace Secyud.Ugf.Modularity
{
    [Registry(
        typeof(IModuleContainer)
    )]
    public class UgfApplication : IUgfApplication
    {
        private readonly Type _startupModuleType;
        private bool _configuredServices;

        public int TotalStep { get; set; }
        public int CurrentStep { get; set; }

        public IDependencyManager DependencyManager { get; }
        public IReadOnlyList<IUgfModuleDescriptor> Modules { get; }

        internal UgfApplication(
            IDependencyManager dependencyManager,
            Type startupModuleType,
            PlugInSourceList plugInSources = null)
        {
            Throw.IfNull(dependencyManager);
            Throw.IfNull(startupModuleType);

            _startupModuleType = startupModuleType;

            DependencyManager = dependencyManager;
            DependencyManager.AddTypes(
                typeof(UgfApplication),
                typeof(ModuleLoader)
            );
            DependencyManager.RegisterInstance<IUgfApplication>(this);
            DependencyManager.RegisterInstance(TypeManager.Instance);

            Modules = LoadModules(DependencyManager, plugInSources);
        }

        public void Configure()
        {
            CheckMultipleConfigureServices();

            ConfigurationContext context = new(DependencyManager);
            
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

            if (!U.DataManager)
            {
                foreach (IUgfModuleDescriptor module in Modules)
                {
                    if (module.Instance is IOnPostConfigure onPost)
                    {
                        onPost.PostConfigure(context);
                    }
                }
            }

            _configuredServices = true;
        }


        public void Shutdown()
        {
            using GameShutDownContext context = new(DependencyManager.CreateScopeProvider());
            
            for (int i = Modules.Count - 1; i >= 0; i--)
            {
                if (Modules[i].Instance is IOnShutDown module)
                    module.OnShutDown(context);
            }
            
            UnityEngine.Application.Quit();
        }


        private IReadOnlyList<IUgfModuleDescriptor> LoadModules(
            IDependencyManager manager,
            PlugInSourceList plugInSources)
        {
            return manager
                .Get<IModuleLoader>()
                .LoadModules(
                    manager,
                    _startupModuleType,
                    plugInSources
                );
        }

        private void CheckMultipleConfigureServices()
        {
            if (_configuredServices)
            {
                throw new UgfInitializationException("Services have already been configured!");
            }
        }
    }
}