#region

using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Modularity
{
    public class ModuleLoader : IModuleLoader
    {
        public IUgfModuleDescriptor[] LoadModules(IDependencyRegistrar registrar,
            Type startupModuleType,
            PlugInSourceList plugInSources)
        {
            Thrower.IfNull(registrar);
            Thrower.IfNull(startupModuleType);

            var modules =
                GetDescriptors(registrar, startupModuleType, plugInSources);

            modules = SortByDependency(modules, startupModuleType);

            return modules.ToArray();
        }


        private List<IUgfModuleDescriptor> GetDescriptors(
            IDependencyRegistrar registrar,
            Type startupModuleType,
            PlugInSourceList plugInSources)
        {
            var modules = new List<UgfModuleDescriptor>();

            FillModules(modules, registrar, startupModuleType, plugInSources);
            SetDependencies(modules);

            return modules.Cast<IUgfModuleDescriptor>().ToList();
        }

        protected virtual void FillModules(List<UgfModuleDescriptor> modules,
            IDependencyRegistrar registrar,
            Type startupModuleType,
            PlugInSourceList plugInSources)
        {
            // startup module
            modules.AddRange(UgfModuleHelper.FindAllModuleTypes(startupModuleType)
                .Select(moduleType => CreateModuleDescriptor(registrar, moduleType)));

            //Plugin modules
            if (plugInSources.IsNullOrEmpty()) return;

            foreach (var moduleType in plugInSources
                         .GetAllModules()
                         .Where(moduleType => modules.All(m => m.Type != moduleType)))
                modules.Add(CreateModuleDescriptor(registrar, moduleType, true));
        }

        protected virtual void SetDependencies(List<UgfModuleDescriptor> modules)
        {
            foreach (var module in modules)
                SetDependencies(modules, module);
        }

        protected virtual List<IUgfModuleDescriptor> SortByDependency(List<IUgfModuleDescriptor> modules,
            Type startupModuleType)
        {
            var sortedModules = modules.SortByDependencies(m => m.Dependencies);
            sortedModules.MoveItem(m => m.Type == startupModuleType, modules.Count - 1);
            return sortedModules;
        }

        protected virtual UgfModuleDescriptor CreateModuleDescriptor(IDependencyRegistrar registrar, Type moduleType,
            bool isLoadedAsPlugIn = false)
        {
            return new UgfModuleDescriptor(moduleType, CreateAndRegisterModule(registrar, moduleType),
                isLoadedAsPlugIn);
        }

        protected virtual IUgfModule CreateAndRegisterModule(IDependencyRegistrar registrar, Type moduleType)
        {
            var module = (IUgfModule)Activator.CreateInstance(moduleType);
            registrar.AddSingleton(moduleType, module);
            return module;
        }

        protected virtual void SetDependencies(List<UgfModuleDescriptor> modules, UgfModuleDescriptor module)
        {
            foreach (var dependedModuleType in UgfModuleHelper.FindDependedModuleTypes(module.Type))
            {
                var dependedModule = modules.FirstOrDefault(m => m.Type == dependedModuleType);
                if (dependedModule == null)
                    throw new UgfException("Could not find a depended module " +
                                           dependedModuleType.AssemblyQualifiedName + " for " +
                                           module.Type.AssemblyQualifiedName);

                module.AddDependency(dependedModule);
            }
        }
    }
}