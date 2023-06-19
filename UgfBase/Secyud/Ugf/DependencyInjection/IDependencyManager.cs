namespace Secyud.Ugf.DependencyInjection
{
    public interface IDependencyManager : IDependencyProvider, IDependencyRegistrar
    {
        public IDependencyProvider CreateScopeProvider();
    }
}