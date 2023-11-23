#region

using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Modularity
{
    public class GameInitializeContext : ModuleContextBase
    {
        public override IDependencyProvider Provider { get; }

        public GameInitializeContext(IDependencyProvider dependencyProvider)
        {
            Throw.IfNull(dependencyProvider);
            Provider = dependencyProvider;
        }
    }
}