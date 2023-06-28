using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity
{
    public class GameShutDownContext : ModuleContextBase
    {
        public override IDependencyProvider Provider { get; }

        public GameShutDownContext(IDependencyProvider dependencyProvider)
        {
            Thrower.IfNull(dependencyProvider);
            Provider = dependencyProvider;
        }
    }
}