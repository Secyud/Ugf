namespace Secyud.Ugf.DependencyInjection
{
    public class DependencyScope : IDependencyScope
    {
        public DependencyScope(IDependencyProvider dependencyProvider)
        {
            DependencyProvider = dependencyProvider;
        }

        public IDependencyProvider DependencyProvider { get; }

        public virtual void Dispose()
        {
        }
    }
}