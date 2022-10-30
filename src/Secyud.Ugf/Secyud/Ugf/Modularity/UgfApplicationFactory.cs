using System;
using System.Threading.Tasks;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity;

public static class UgfApplicationFactory
{
    public static async Task<IUgfApplication> CreateAsync<TStartupModule>() where TStartupModule : IUgfModule
    {
        return await CreateAsync(typeof(TStartupModule));
    }

    public static async Task<IUgfApplication> CreateAsync(Type startupModuleType)
    {
        var app = Create(startupModuleType);
        await app.ConfigureAsync();
        return app;
    }


    public static IUgfApplication Create<TStartupModule>()
        where TStartupModule : IUgfModule
    {
        return Create(typeof(TStartupModule));
    }

    public static IUgfApplication Create(Type startupModuleType)
    {
        return new UgfApplication(new DependencyManager(), startupModuleType);
    }
}