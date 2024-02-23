using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity
{
    public class UgfApplicationFactory
    {
        public IUgfApplication Application { get; }
        private bool _configured = false;

        public UgfApplicationFactory(Type startUpModule, PlugInSourceList plugInSources = null)
        {
            List<IUgfModuleDescriptor> moduleDescriptors = ModuleLoader.LoadModules(startUpModule, plugInSources);
            Application = new UgfApplication(new DependencyManager(), moduleDescriptors);
        }

        public void Shutdown()
        {
            Application.Shutdown();
        }

        public void Configure()
        {
            if (_configured) return;
            Application.Configure();
            _configured = true;
        }

        public void ConfigureDataManager()
        {
            if (_configured) return;

            Application.ConfigureDataManager();

            _configured = true;
        }
    }
}