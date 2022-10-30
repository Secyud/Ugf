using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity;

public class ShutdownContext : IDependencyProviderAccessor
{
    public ShutdownContext(IDependencyProvider dependencyProvider)
    {
        Thrower.IfNull(dependencyProvider);

        DependencyProvider = dependencyProvider;
    }

    public IDependencyProvider DependencyProvider { get; }
}