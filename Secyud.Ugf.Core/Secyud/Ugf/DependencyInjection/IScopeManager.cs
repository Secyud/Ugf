namespace Secyud.Ugf.DependencyInjection
{
    public interface IScopeManager
    {
        public IDependencyProvider CreateScopeProvider();
        TScope CreateScope<TScope>() where TScope : DependencyScopeProvider;
        void DestroyScope<TScope>() where TScope : DependencyScopeProvider;
        TScope GetScope<TScope>() where TScope : DependencyScopeProvider;
    }
}