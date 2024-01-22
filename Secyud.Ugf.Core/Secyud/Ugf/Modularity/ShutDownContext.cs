using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity
{
    public class ShutDownContext 
    {
        public IDependencyProvider Provider { get; }

        public ShutDownContext(IDependencyProvider dependencyProvider)
        {
            Throw.IfNull(dependencyProvider);
            Provider = dependencyProvider;
        }
    }
}