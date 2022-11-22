using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity
{
    public class InitializationContext : IDependencyProviderAccessor
    {
        public InitializationContext(IDependencyProvider dependencyProvider)
        {
            Thrower.IfNull(dependencyProvider);
            DependencyProvider = dependencyProvider;
        }

        public IDependencyProvider DependencyProvider { get; }
    }
}