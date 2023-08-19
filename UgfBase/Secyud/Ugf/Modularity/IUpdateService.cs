using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.Modularity
{
    public interface IUpdateService:IRegistry
    {
        void Update();
    }
}