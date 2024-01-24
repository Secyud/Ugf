namespace Secyud.Ugf.Modularity
{
    public interface IUgfApplication : IModuleContainer
    {
        void Configure();
        void Shutdown();
    }
}