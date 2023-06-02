namespace Secyud.Ugf.DependencyInjection
{
    public class DependencyScope : IDependencyScope
    {
        public IDependencyProvider DependencyProvider => Provider;

        internal readonly DependencyProvider Provider;

        public T Get<T>() where T : class => DependencyProvider.Get<T>();

        public DependencyScope(IDependencyProviderFactory dependencyProvider)
        {
            Provider = new DependencyProvider(dependencyProvider);
        }


        public virtual void Dispose()
        {
        }
    }
}