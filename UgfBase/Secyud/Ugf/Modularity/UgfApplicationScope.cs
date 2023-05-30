using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity
{
    public class UgfApplicationScope:DependencyScope
    {
        public UgfApplicationScope(IDependencyProvider dependencyProvider) : base(dependencyProvider)
        {
        }
    }
}