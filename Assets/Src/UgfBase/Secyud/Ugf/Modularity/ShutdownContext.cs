#region

using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Modularity
{
    public class ShutdownContext : IDependencyProviderAccessor
    {
        public ShutdownContext(IDependencyProvider dependencyProvider)
        {
            Thrower.IfNull(dependencyProvider);

            DependencyProvider = dependencyProvider;
        }

        public IDependencyProvider DependencyProvider { get; }
        public T Get<T>() where T : class => DependencyProvider.Get<T>();
    }
}