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

        public int TotalStep { get; private set; }
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

            List<IPreConfigure> onPre = new();
            List<IPostConfigure> onPost = new();

            foreach (IUgfModuleDescriptor descriptor in Modules)
            {
                if (descriptor.Instance is IPreConfigure preInitialization)
                    onPre.AddLast(preInitialization);
                if (descriptor.Instance is IPostConfigure postInitialization)
                    onPost.AddLast(postInitialization);
            }


            foreach (IPreConfigure module in onPre)
                module.PreConfigureGame(context);

            foreach (IUgfModuleDescriptor module in Modules)
                module.Instance.ConfigureGame(context);

            if (!U.DataManager)
                foreach (IPostConfigure module in onPost)
                    module.PostConfigureGame(context);

            _configuredServices = true;
        }

        public IEnumerator GameInitialization()
        {
            using GameInitializeContext context = new(DependencyManager.CreateScopeProvider());

            List<IOnPreInitialization> onPreInitializations = new();
            List<IOnInitialization> onInitializations = new();
            List<IOnPostInitialization> onPostInitializations = new();

            foreach (IUgfModuleDescriptor descriptor in Modules)
            {
                if (descriptor.Instance is IOnPreInitialization preInitialization)
                    onPreInitializations.AddLast(preInitialization);
                if (descriptor.Instance is IOnInitialization initialization)
                    onInitializations.AddLast(initialization);
                if (descriptor.Instance is IOnPostInitialization postInitialization)
                    onPostInitializations.AddLast(postInitialization);
            }

            foreach (IOnPreInitialization module in onPreInitializations)
                yield return module.OnGamePreInitialization(context);

            foreach (IOnInitialization module in onInitializations)
                yield return module.OnGameInitializing(context);

            foreach (IOnPostInitialization module in onPostInitializations)
                yield return module.OnGamePostInitialization(context);
        }

        public IEnumerator GameSaving()
        {
            foreach (IUgfModuleDescriptor descriptor in Modules)
                if (descriptor.Instance is IOnArchiving archiving)
                    yield return archiving.SaveGame();
        }

        public void Shutdown()
        {
            using GameShutDownContext context = new(DependencyManager.CreateScopeProvider());

            for (int i = Modules.Count - 1; i >= 0; i--)
            {
                if (Modules[i].Instance is IOnShutDown module)
                    module.OnGameShutDown(context);
            }
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
                throw new UgfInitializationException("Services have already been configured!");
        }
    }
}