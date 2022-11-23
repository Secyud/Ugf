using System;
using System.Threading.Tasks;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity;

public interface IUgfApplication : IModuleContainer, IDisposable
{
    Type StartupModuleType { get; }

    IDependencyProvider DependencyProvider { get; }

    IDependencyScope CreateDependencyScope();

    Task ConfigureAsync();

    Task InitializeAsync();

    Task ShutdownAsync();
}