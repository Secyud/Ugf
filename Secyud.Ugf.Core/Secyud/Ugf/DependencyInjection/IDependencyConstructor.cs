namespace Secyud.Ugf.DependencyInjection
{
    public interface IDependencyConstructor
    {
        object Construct(IDependencyProvider provider);
    }
}