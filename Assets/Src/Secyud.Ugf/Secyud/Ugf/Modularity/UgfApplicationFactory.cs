using System;
using System.Threading.Tasks;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity
{
    public static class UgfApplicationFactory
    {
        public static async Task<IUgfApplication> CreateAsync<TStartupModule>(
            PlugInSourceList plugInSources = null)
            where TStartupModule : IUgfModule
        {
            return await CreateAsync(typeof(TStartupModule), plugInSources);
        }

        public static async Task<IUgfApplication> CreateAsync(
            Type startupModuleType,
            PlugInSourceList plugInSources = null)
        {
            var app = Create(startupModuleType, plugInSources);
            await app.ConfigureAsync();
            return app;
        }

        private static IUgfApplication Create(
            Type startupModuleType,
            PlugInSourceList plugInSources = null)
        {
            return new UgfApplication(new DependencyManager(), startupModuleType, plugInSources);
        }
    }
}