using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity
{
    public class UgfApplication : IUgfApplication
    {
        private readonly IDependencyManager _dependencyManager;

        private bool _configuredServices;

        internal UgfApplication(
            IDependencyManager dependencyManager,
            Type startupModuleType,
            PlugInSourceList plugInSources = null)
        {
            Thrower.IfNull(dependencyManager);
            Thrower.IfNull(startupModuleType);

            StartupModuleType = startupModuleType;

            _dependencyManager = dependencyManager;
            _dependencyManager.AddSingleton<IUgfApplication>(this);
            _dependencyManager.AddSingleton<IModuleContainer>(this);
            _dependencyManager.AddSingleton<IModuleLoader>(new ModuleLoader());

            Modules = LoadModules(_dependencyManager, plugInSources);
        }

        public Type StartupModuleType { get; }
        public IDependencyProvider DependencyProvider => _dependencyManager;

        public IReadOnlyList<IUgfModuleDescriptor> Modules { get; }

        public IDependencyScope CreateDependencyScope()
        {
            return _dependencyManager.CreateScope();
        }

        public async Task ConfigureAsync()
        {
            CheckMultipleConfigureServices();

            var context = new ConfigurationContext(_dependencyManager);

            try
            {
                var ugfModules =
                    Modules
                        .Where(m => m.Instance is UgfModule)
                        .Select(m => m.Instance as UgfModule)
                        .ToArray();
                
                foreach (var module in ugfModules)
                    module!.ConfigurationContext = context;

                foreach (var module in Modules.Where(m => m.Instance is IPreConfigure))
                    await ((IPreConfigure)module.Instance).PreConfigureGameAsync(context);

                foreach (var module in Modules)
                    await module.Instance.ConfigureGameAsync(context);

                foreach (var module in Modules.Where(m => m.Instance is IPostConfigure))
                    await ((IPostConfigure)module.Instance).PostConfigureGameAsync(context);

                foreach (var module in ugfModules)
                    module!.ConfigurationContext = null;
            }
            catch (Exception ex)
            {
                throw new UgfInitializationException(
                    $"An error occurred during {nameof(ConfigureAsync)}. See the inner exception for details.", ex);
            }

            _configuredServices = true;
        }

        public async Task InitializeAsync()
        {
            try
            {
                using var scope = CreateDependencyScope();

                var context = new InitializationContext(scope.DependencyProvider);

                foreach (var module in Modules.Where(m => m.Instance is IOnPreInitialization))
                    await ((IOnPreInitialization)module.Instance).OnGamePreInitializationAsync(context);


                foreach (var module in Modules.Where(m => m.Instance is IOnInitialization))
                    await ((IOnInitialization)module.Instance).OnGameInitializationAsync(context);


                foreach (var module in Modules.Where(m => m.Instance is IOnPostInitialization))
                    await ((IOnPostInitialization)module.Instance).OnGamePostInitializationAsync(context);
            }
            catch (Exception ex)
            {
                throw new UgfInitializationException(
                    $"An error occurred during {nameof(InitializeAsync)}. See the inner exception for details.", ex);
            }
        }

        public async Task ShutdownAsync()
        {
            try
            {
                using var scope = CreateDependencyScope();

                var context = new ShutdownContext(scope.DependencyProvider);

                for (var i = Modules.Count - 1; i >= 0; --i)
                    if (Modules[i].Instance is IOnShutdown)
                        await ((IOnShutdown)Modules[i].Instance).OnGameShutdownAsync(context);
                Dispose();
            }
            catch (Exception ex)
            {
                throw new UgfInitializationException(
                    $"An error occurred during {nameof(InitializeAsync)}. See the inner exception for details.", ex);
            }
        }

        public void Dispose()
        {
        }

        private IReadOnlyList<IUgfModuleDescriptor> LoadModules(
            IDependencyManager manager,
            PlugInSourceList plugInSources)
        {
            return manager
                .Get<IModuleLoader>()
                .LoadModules(
                    manager,
                    StartupModuleType,
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