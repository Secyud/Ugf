#region

using Secyud.Ugf.DependencyInjection;

#endregion

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

        public T Get<T>() where T : class
        {
            return DependencyProvider.Get<T>();
        }
    }
}